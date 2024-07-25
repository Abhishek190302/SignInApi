using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class EnquiryListingRepository
    {
        private readonly string _connectionString;
        public EnquiryListingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<Listing> GetListingByOwnerIdAsync(string ownerId)
        {
            Listing listing = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT ListingID, OwnerGuid FROM [listing].[Listing] WHERE OwnerGuid = @OwnerId", conn);
                cmd.Parameters.AddWithValue("@OwnerId", ownerId);

                using (var adapter = new SqlDataAdapter(cmd))
                {
                    var dataTable = new DataTable();
                    await conn.OpenAsync();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow row = dataTable.Rows[0];
                        listing = new Listing
                        {
                            Listingid = Convert.ToInt32(row["ListingID"]),
                            OwnerGuid = row["OwnerGuid"].ToString(),
                            // Add other properties here
                        };
                    }
                }
            }

            return listing;
        }


        public async Task<IList<ListingEnquiry>> GetEnquiryByListingIdAsync(int listingId)
        {
            List<ListingEnquiry> enquiries = new List<ListingEnquiry>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Enquiry] WHERE ListingID = @ListingId ORDER BY CreatedDate DESC", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);

                using (var adapter = new SqlDataAdapter(cmd))
                {
                    var dataTable = new DataTable();
                    await conn.OpenAsync();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        enquiries.Add(new ListingEnquiry
                        {
                            EnquiryID = Convert.ToInt32(row["Id"]),
                            ListingID = Convert.ToInt32(row["ListingID"]),
                            CreatedDate = Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-dd hh:mm:ss tt"),
                            OwnerGuid = row["OwnerGuid"].ToString(),
                            FullName = row["FullName"].ToString(),
                            Email = row["Email"].ToString(),
                            MobileNumber = row["MobileNumber"].ToString(),
                            EnquiryTitle = row["EnquiryTitle"].ToString(),
                            Message = row["Message"].ToString(),
                            // Add other properties here
                        });
                    }
                }
            }

            return enquiries;
        }

    }
}
