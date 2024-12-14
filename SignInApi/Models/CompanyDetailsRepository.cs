using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml.Linq;
using Twilio.TwiML.Voice;

namespace SignInApi.Models
{
    public class CompanyDetailsRepository
    {
        private readonly string _connectionString;
        private readonly string _connectionCategoryString;

        public CompanyDetailsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionCategoryString = configuration.GetConnectionString("MimCategories");
        }

        public async Task<Listing> GetListingByOwnerIdAsync(dynamic ownerId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE OwnerGuid = @OwnerGuid", conn);
                    cmd.Parameters.AddWithValue("@OwnerGuid", ownerId);
                    await conn.OpenAsync();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        return new Listing
                        {
                            Listingid = row.Field<int?>("ListingID") ?? 0,
                            OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                            CreatedDate = row.Field<DateTime?>("CreatedDate") ?? default(DateTime),
                            CreatedTime = row.Field<DateTime?>("CreatedTime") ?? default(DateTime),
                            IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                            Status = row.Field<int?>("Status") ?? 0,
                            Name = row.Field<string>("Name") ?? string.Empty,
                            LastName = row.Field<string>("LastName") ?? string.Empty,
                            Gender = row.Field<string>("Gender") ?? string.Empty,
                            Designation = row.Field<string>("Designation") ?? string.Empty,
                            ListingURL = row.Field<string>("ListingURL") ?? string.Empty,
                            ApprovedOrRejectedBy = row.Field<bool?>("ApprovedOrRejectedBy") ?? false,
                            Rejected = row.Field<bool?>("Rejected") ?? false,
                            Steps = row.Field<int?>("Steps") ?? 0,
                            Id = row.Field<Guid?>("Id") ?? Guid.Empty,
                            ClaimedListing = row.Field<bool?>("ClaimedListing") ?? false,
                            SelfCreated = row.Field<bool?>("SelfCreated") ?? false
                        };
                    }
                    return null;
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
        }


        public async Task<Listing> GetListingByIdAsync(int listingid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE ListingID = @ListingID", conn);
                    cmd.Parameters.AddWithValue("@ListingID", listingid);
                    await conn.OpenAsync();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        var listing = new Listing
                        {
                            Listingid = row.Field<int?>("ListingID") ?? 0,
                            OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                            CreatedDate = row.Field<DateTime?>("CreatedDate") ?? default(DateTime),
                            CreatedTime = row.Field<DateTime?>("CreatedTime") ?? default(DateTime),
                            IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                            Status = row.Field<int?>("Status") ?? 0,
                            Name = row.Field<string>("Name") ?? string.Empty,
                            LastName = row.Field<string>("LastName") ?? string.Empty,
                            Gender = row.Field<string>("Gender") ?? string.Empty,
                            Designation = row.Field<string>("Designation") ?? string.Empty,
                            ListingURL = row.Field<string>("ListingURL") ?? string.Empty,
                            ApprovedOrRejectedBy = row.Field<bool?>("ApprovedOrRejectedBy") ?? false,
                            Rejected = row.Field<bool?>("Rejected") ?? false,
                            Steps = row.Field<int?>("Steps") ?? 0,
                            Id = row.Field<Guid?>("Id") ?? Guid.Empty,
                            ClaimedListing = row.Field<bool?>("ClaimedListing") ?? false,
                            SelfCreated = row.Field<bool?>("SelfCreated") ?? false
                        };
                        return listing;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<Listing> AddListingAsync(Listing listing)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Usp_Companydetails", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    string formattedListingURL = listing.CompanyName.Replace(' ', '-');

                    cmd.Parameters.AddWithValue("@OwnerGuid", listing.OwnerGuid ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", listing.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedDate", listing.CreatedDate);
                    cmd.Parameters.AddWithValue("@CreatedTime", listing.CreatedTime);
                    cmd.Parameters.AddWithValue("@Name", listing.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", listing.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyName", listing.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@YearOfEstablishment", listing.YearOfEstablishment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumberOfEmployees", listing.NumberOfEmployees ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Designation", listing.Designation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NatureOfBusiness", listing.NatureOfBusiness ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Turnover", listing.Turnover ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ListingURL", formattedListingURL ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApprovedOrRejectedBy", listing.ApprovedOrRejectedBy);
                    cmd.Parameters.AddWithValue("@Rejected", listing.Rejected);
                    cmd.Parameters.AddWithValue("@GSTNumber", listing.GSTNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Steps", listing.Steps);
                    cmd.Parameters.AddWithValue("@BusinessCategory", listing.BusinessCategory ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", listing.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", listing.Id);
                    cmd.Parameters.AddWithValue("@Status", listing.Status);
                    cmd.Parameters.AddWithValue("@ClaimedListing", listing.ClaimedListing);
                    cmd.Parameters.AddWithValue("@SelfCreated", listing.SelfCreated);
                    cmd.Parameters.AddWithValue("@LastName", listing.LastName ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();     
                }
            }
            catch(Exception ex)
            {

            }
            return listing;
        }

        public async Task<Listing> UpdateListingAsync(Listing listing)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("Usp_CompanydetailsUpdate", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", listing.Listingid);
                    cmd.Parameters.AddWithValue("@OwnerGuid", listing.OwnerGuid);
                    cmd.Parameters.AddWithValue("@IPAddress", listing.IPAddress);
                    cmd.Parameters.AddWithValue("@CreatedDate", listing.CreatedDate);
                    cmd.Parameters.AddWithValue("@CreatedTime", listing.CreatedTime);
                    cmd.Parameters.AddWithValue("@Name", listing.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", listing.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyName", listing.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@YearOfEstablishment", listing.YearOfEstablishment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumberOfEmployees", listing.NumberOfEmployees ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Designation", listing.Designation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NatureOfBusiness", listing.NatureOfBusiness ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Turnover", listing.Turnover ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ListingURL", listing.ListingURL ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApprovedOrRejectedBy", listing.ApprovedOrRejectedBy);
                    cmd.Parameters.AddWithValue("@Rejected", listing.Rejected);
                    cmd.Parameters.AddWithValue("@GSTNumber", listing.GSTNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Steps", listing.Steps);
                    cmd.Parameters.AddWithValue("@BusinessCategory", listing.BusinessCategory ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", listing.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", listing.Id);
                    cmd.Parameters.AddWithValue("@Status", listing.Status);
                    cmd.Parameters.AddWithValue("@ClaimedListing", listing.ClaimedListing);
                    cmd.Parameters.AddWithValue("@SelfCreated", listing.SelfCreated);
                    cmd.Parameters.AddWithValue("@LastName", listing.LastName ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch(Exception ex)
            {

            }
            return listing;
        }

        public List<string> GetDistinctKeywords()
        {
            var keywords = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT DISTINCT SeoKeyword FROM [dbo].[Keyword]", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);                   
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    keywords.Add(row["SeoKeyword"].ToString());
                } 
            }

            using (SqlConnection connection = new SqlConnection(_connectionCategoryString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT SearchKeywordName FROM [cat].[SecondCategory]", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    keywords.Add(row["SearchKeywordName"].ToString());
                }
            }

            return keywords;
        }
    }
}
