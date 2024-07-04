using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class SocialNetworkRepository
    {
        private readonly string _connectionString;
        public SocialNetworkRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<SocialNetwork> GetSocialNetworkByListingId(int listingId)
        {
            SocialNetwork socialNetwork = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[SocialNetwork] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        socialNetwork = new SocialNetwork
                        {
                            ListingID = (int)row["ListingID"],
                            OwnerGuid = row["OwnerGuid"].ToString(),
                            IPAddress = row["IPAddress"].ToString(),
                            Facebook = row["Facebook"].ToString(),
                            WhatsappGroupLink = row["WhatsappGroupLink"].ToString(),
                            Linkedin = row["Linkedin"].ToString(),
                            Twitter = row["Twitter"].ToString(),
                            Youtube = row["Youtube"].ToString(),
                            Instagram = row["Instagram"].ToString(),
                            Pinterest = row["Pinterest"].ToString()
                        };
                    }
                }
            }

            return socialNetwork;
        }

        public async Task AddAsync(SocialNetwork socialNetwork)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Usp_SocialNetwork", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ListingID", socialNetwork.ListingID);
                cmd.Parameters.AddWithValue("@OwnerGuid", socialNetwork.OwnerGuid);
                cmd.Parameters.AddWithValue("@IPAddress", socialNetwork.IPAddress);
                cmd.Parameters.AddWithValue("@Facebook", socialNetwork.Facebook);
                cmd.Parameters.AddWithValue("@WhatsappGroupLink", socialNetwork.WhatsappGroupLink);
                cmd.Parameters.AddWithValue("@Linkedin", socialNetwork.Linkedin);
                cmd.Parameters.AddWithValue("@Twitter", socialNetwork.Twitter);
                cmd.Parameters.AddWithValue("@Youtube", socialNetwork.Youtube);
                cmd.Parameters.AddWithValue("@Instagram", socialNetwork.Instagram);
                cmd.Parameters.AddWithValue("@Pinterest", socialNetwork.Pinterest);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(SocialNetwork socialNetwork)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Usp_UpdateSocialNetwork", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ListingID", socialNetwork.ListingID);
                cmd.Parameters.AddWithValue("@OwnerGuid", socialNetwork.OwnerGuid);
                cmd.Parameters.AddWithValue("@IPAddress", socialNetwork.IPAddress);
                cmd.Parameters.AddWithValue("@Facebook", socialNetwork.Facebook);
                cmd.Parameters.AddWithValue("@WhatsappGroupLink", socialNetwork.WhatsappGroupLink);
                cmd.Parameters.AddWithValue("@Linkedin", socialNetwork.Linkedin);
                cmd.Parameters.AddWithValue("@Twitter", socialNetwork.Twitter);
                cmd.Parameters.AddWithValue("@Youtube", socialNetwork.Youtube);
                cmd.Parameters.AddWithValue("@Instagram", socialNetwork.Instagram);
                cmd.Parameters.AddWithValue("@Pinterest", socialNetwork.Pinterest);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
