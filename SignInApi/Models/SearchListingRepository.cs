using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SignInApi.Models
{
    public class SearchListingRepository
    {
        private readonly string _connectionListing;
        private readonly string _connectionShared;

        public SearchListingRepository(IConfiguration configuration)
        {
            _connectionListing = configuration.GetConnectionString("MimListing");
            _connectionShared = configuration.GetConnectionString("MimShared");
        }

        public async Task<IList<ListingSearch>> GetApprovedListings()
        {
            var listings = new List<ListingSearch>();

            using (SqlConnection conn = new SqlConnection(_connectionListing))
            {
                await conn.OpenAsync();
                string query = @"
                SELECT l.ListingID, l.CompanyName, l.ListingURL, a.AssemblyID 
                FROM [listing].[Listing] l
                JOIN [listing].[Address] a ON l.ListingID = a.ListingID
                WHERE l.Status = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    listings.Add(new ListingSearch
                    {
                        ListingId = row["ListingID"].ToString(),
                        CompanyName = row["CompanyName"].ToString(),
                        ListingURL = row["ListingURL"].ToString(),
                        Address = new AddressSearch { AssemblyID = row["AssemblyID"].ToString() }
                    });
                }
            }

            return listings;
        }


        public async Task<IList<LocalitySearch>> GetLocalitiesByLocalityIds(string[] localityIds)
        {
            try
            {
                var localities = new List<LocalitySearch>();

                using (SqlConnection conn = new SqlConnection(_connectionShared))
                {
                    await conn.OpenAsync();
                    var queryBuilder = new StringBuilder();
                    queryBuilder.Append(@"
                    SELECT l.Id, l.Name, c.Name as CityName 
                    FROM [dbo].[Location] l 
                    JOIN [shared].[City] c ON l.CityID = c.CityID 
                    WHERE l.Id IN (");

                    var parameters = new List<SqlParameter>();

                    for (int i = 0; i < localityIds.Length; i++)
                    {
                        var paramName = "@localityId" + i;
                        if (i > 0)
                        {
                            queryBuilder.Append(",");
                        }
                        queryBuilder.Append(paramName);
                        parameters.Add(new SqlParameter(paramName, localityIds[i]));
                    }

                    queryBuilder.Append(")");

                    SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn);
                    cmd.Parameters.AddRange(parameters.ToArray());
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        localities.Add(new LocalitySearch
                        {
                            Id = row["Id"].ToString(),
                            Name = row["Name"].ToString(),
                            City = new CitySearch { Name = row["CityName"].ToString() }
                        });
                    }
                }

                return localities;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
