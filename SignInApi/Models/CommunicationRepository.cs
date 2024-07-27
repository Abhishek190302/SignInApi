using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class CommunicationRepository
    {
        private readonly string _connectionString;
        public CommunicationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<Communication> GetCommunicationByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Communication] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Communication
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        Email = row.Field<string>("Email") ?? string.Empty,
                        Mobile = row.Field<string>("Mobile") ?? string.Empty,
                        Telephone = row.Field<string>("Telephone") ?? string.Empty,
                        TelephoneSecond = row.Field<string>("TelephoneSecond") ?? string.Empty,
                        Website = row.Field<string>("Website") ?? string.Empty,
                        TollFree = row.Field<string>("TollFree") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                        Language = row.Field<string>("Language") ?? string.Empty
                    };
                }
                return null;
            }
        }

        public async Task AddCommunicationAsync(Communication communication)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Usp_Communication", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", communication.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", communication.OwnerGuid ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", communication.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", communication.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Website", communication.Website ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", communication.Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Whatsapp", communication.Whatsapp ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telephone", communication.Telephone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TollFree", communication.TollFree ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", communication.Fax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SkypeID", communication.SkypeID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneSecond", communication.TelephoneSecond ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Language", communication.Language ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                { 
                    throw; 
                }
            }
        }

        public async Task UpdateCommunicationAsync(Communication communication)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Usp_CommunicationUpdate", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ListingID", communication.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", communication.OwnerGuid ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", communication.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", communication.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Website", communication.Website ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", communication.Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Whatsapp", communication.Whatsapp ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telephone", communication.Telephone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TollFree", communication.TollFree ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", communication.Fax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SkypeID", communication.SkypeID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneSecond", communication.TelephoneSecond ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Language", communication.Language ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
