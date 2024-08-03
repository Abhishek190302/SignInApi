using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class ListingEnquiryService
    {
        private readonly string _connectionString;
        private readonly string _connectionMimUser;
        private readonly string _configuration;
        public ListingEnquiryService(IConfiguration configuration)
        {
             _connectionString = configuration.GetConnectionString("MimListing");
            _connectionMimUser = configuration.GetConnectionString("MimUser");
        }

        public async Task<UserProfile> GetProfileByOwnerGuid(string ownerGuid)
        {
            UserProfile userProfile = null;
            using (SqlConnection conn = new SqlConnection(_connectionMimUser))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid", conn);
                cmd.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    userProfile = new UserProfile
                    {
                        OwnerGuid = row["OwnerGuid"].ToString(),
                        IsProfileCompleted = Convert.ToBoolean(row["IsProfileCompleted"])
                    };
                }
            }

            return userProfile;
        }

        public async Task AddAsync(ListingEnquiry enquiry)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [dbo].[Enquiry] (ListingID, OwnerGuid, FullName, Email, MobileNumber, EnquiryTitle, Message,CreatedDate) VALUES (@ListingID, @OwnerGuid, @FullName, @Email, @MobileNumber, @EnquiryTitle, @Message,GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ListingID", enquiry.ListingID);
                cmd.Parameters.AddWithValue("@OwnerGuid", enquiry.OwnerGuid);
                cmd.Parameters.AddWithValue("@FullName", enquiry.FullName);
                cmd.Parameters.AddWithValue("@Email", enquiry.Email);
                cmd.Parameters.AddWithValue("@MobileNumber", enquiry.MobileNumber);
                cmd.Parameters.AddWithValue("@EnquiryTitle", enquiry.EnquiryTitle);
                cmd.Parameters.AddWithValue("@Message", enquiry.Message);

                conn.Open();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(ListingEnquiry enquiry)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = "UPDATE [dbo].[Enquiry] " +
                                   "SET OwnerGuid = @OwnerGuid, " +
                                   "FullName = @FullName, " +
                                   "Email = @Email, " +
                                   "MobileNumber = @MobileNumber, " +
                                   "EnquiryTitle = @EnquiryTitle, " +
                                   "Message = @Message, " +
                                   "CreatedDate = GETDATE() " +
                                   "WHERE ListingID = @ListingID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OwnerGuid", enquiry.OwnerGuid);
                    cmd.Parameters.AddWithValue("@FullName", enquiry.FullName);
                    cmd.Parameters.AddWithValue("@Email", enquiry.Email);
                    cmd.Parameters.AddWithValue("@MobileNumber", enquiry.MobileNumber);
                    cmd.Parameters.AddWithValue("@EnquiryTitle", enquiry.EnquiryTitle);
                    cmd.Parameters.AddWithValue("@Message", enquiry.Message);
                    cmd.Parameters.AddWithValue("@ListingID", enquiry.ListingID);

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    // Log the exception here or handle it as required
                    throw;
                }
            }
        }


        public async Task<ListingEnquiry> GetEnquiryListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Enquiry] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new ListingEnquiry
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        FullName = row.Field<string>("FullName") ?? string.Empty,
                        Email = row.Field<string>("Email") ?? string.Empty,
                        MobileNumber = row.Field<string>("MobileNumber") ?? string.Empty,
                        EnquiryTitle = row.Field<string>("EnquiryTitle") ?? string.Empty,
                        Message = row.Field<string>("Message") ?? string.Empty,
                    };
                }
                return null;
            }
        }
    }
}
