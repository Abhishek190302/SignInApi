using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class DashboardRepository
    {
        private readonly string _connectionString;
        private readonly string _connectionAudit;
        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionAudit = configuration.GetConnectionString("AuditTrail");
        }

        public async Task<Listing> GetListingByOwnerId(string OwnerGuid)
        {
            Listing listing = new Listing();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                SqlCommand cmd = new SqlCommand("SELECT TOP 1 ListingID FROM [listing].[Listing] WHERE OwnerGuid = @OwnerId", conn);
                cmd.Parameters.AddWithValue("@OwnerId", OwnerGuid);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Listing
                    {
                        Listingid = row.Field<int?>("ListingID") ?? 0
                    };
                }
                return null;
            }
        }

        public async Task<int> GetLikesByListingId(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionAudit))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [audit].[LikeDislike] WHERE ListingID = @ListingID AND [Like] = 1", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
           
        }

        public async Task<int> GetBookmarksByListingId(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionAudit))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [audit].[Bookmarks] WHERE ListingID = @ListingID AND Bookmark = 1", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

        public async Task<int> GetSubscribersByListingId(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionAudit))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [audit].[Subscribes] WHERE ListingID = @ListingID AND Subscribe = 1", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
            
        }

        public async Task<int> GetRatingsByListingId(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [listing].[Rating] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

        public async Task<int> GetLikesByOwnerId(string ownerid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionAudit))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [audit].[LikeDislike] WHERE UserGuid = @UserGuid AND [Like] = 1", conn);
                cmd.Parameters.AddWithValue("@UserGuid", ownerid);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

        }

        public async Task<int> GetBookmarksByOwnerId(string ownerid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionAudit))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [audit].[Bookmarks] WHERE UserGuid = @UserGuid AND Bookmark = 1", conn);
                cmd.Parameters.AddWithValue("@UserGuid", ownerid);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

        public async Task<int> GetSubscribersByOwnerId(string ownerid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionAudit))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [audit].[Subscribes] WHERE UserGuid = @UserGuid AND Subscribe = 1", conn);
                cmd.Parameters.AddWithValue("@UserGuid", ownerid);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

        public async Task<int> GetRatingsByOwnerId(string ownerid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [listing].[Rating] WHERE OwnerGuid = @OwnerGuid", conn);
                cmd.Parameters.AddWithValue("@OwnerGuid", ownerid);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }
    }
}
