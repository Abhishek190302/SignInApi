using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class KeywordRepository
    {
        private readonly string _connectionString;
        public KeywordRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
        }

        public async Task<bool> KeywordExists(string seoKeyword)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT COUNT(*) FROM [dbo].[Keyword] WHERE SeoKeyword = @SeoKeyword";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SeoKeyword", seoKeyword);
                    await connection.OpenAsync();
                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task RemoveKeywordsAsync(List<Keyword> keywords)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var keyword in keywords)
                {
                    var query = "DELETE FROM [dbo].[Keyword] WHERE ListingID = @ListingID AND OwnerGuid = @OwnerGuid AND SeoKeyword = @SeoKeyword";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ListingID", keyword.ListingID);
                        command.Parameters.AddWithValue("@OwnerGuid", keyword.OwnerGuid);
                        command.Parameters.AddWithValue("@SeoKeyword", keyword.SeoKeyword);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task SaveKeywordsAsync(List<Keyword> keywords)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var keyword in keywords)
                {
                    var query = "INSERT INTO [dbo].[Keyword] (ListingID, OwnerGuid, SeoKeyword) VALUES (@ListingID, @OwnerGuid, @SeoKeyword)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ListingID", keyword.ListingID);
                        command.Parameters.AddWithValue("@OwnerGuid", keyword.OwnerGuid);
                        command.Parameters.AddWithValue("@SeoKeyword", keyword.SeoKeyword);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task<List<Keyword>> GetKeywordsByListingIdAsync(int listingId)
        {
            var keywords = new List<Keyword>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("SELECT ListingID, OwnerGuid, SeoKeyword FROM [dbo].[Keyword] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        keywords.Add(new Keyword
                        {
                            ListingID = (int)row["ListingID"],
                            OwnerGuid = (string)row["OwnerGuid"],
                            SeoKeyword = (string)row["SeoKeyword"]
                        });
                    }
                }
            }

            return keywords;
        }
    }
}
