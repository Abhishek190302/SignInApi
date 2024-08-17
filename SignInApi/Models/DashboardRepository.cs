using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Models
{
    public class DashboardRepository
    {
        private readonly string _connectionString;
        private readonly string _connectionAudit;
        private readonly string _connectionUser;
        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionAudit = configuration.GetConnectionString("AuditTrail");
            _connectionUser = configuration.GetConnectionString("MimUser");
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

        public async Task<Listing> GetListingByOwnerIdAsync(string ownerId)
        {
            Listing listing = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE OwnerGuid = @OwnerId", connection);
                command.Parameters.AddWithValue("@OwnerId", ownerId);
                var da = new SqlDataAdapter(command);
                var dt = new DataTable();
                await connection.OpenAsync();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    listing = new Listing
                    {
                        Listingid = Convert.ToInt32(row["ListingID"]),
                        CompanyName = row["CompanyName"].ToString()
                    };
                }
            }
            return listing;
        }

        public async Task<IEnumerable<ListingActivity>> GetLikesByListingIdAsync(int listingId)
        {
            return await GetListingActivitiesAsync("SELECT * FROM [audit].[LikeDislike] WHERE ListingID = @ListingID", listingId);
        }

        public async Task<IEnumerable<ListingActivity>> GetBookmarksByListingIdAsync(int listingId)
        {
            return await GetListingActivitiesAsync("SELECT * FROM [audit].[Bookmarks] WHERE ListingID = @ListingID", listingId);
        }

        public async Task<IEnumerable<ListingActivity>> GetSubscribersByListingIdAsync(int listingId)
        {
            return await GetListingActivitiesAsync("SELECT * FROM [audit].[Subscribes] WHERE ListingID = @ListingID", listingId);
        }

        public async Task<IEnumerable<ListingActivity>> GetEnquiryByListingIdAsync(int listingId)
        {
            return await GetEnquiryListingActivitiesAsync("SELECT * FROM [dbo].[Enquiry] WHERE ListingID = @ListingId ORDER BY CreatedDate DESC", listingId);
        }



        public async Task<IEnumerable<ListingActivity>> GetListingActivitiesAsync(string query, int listingId)
        {
            var activities = new List<ListingActivity>();

            using (var connection = new SqlConnection(_connectionAudit))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ListingID", listingId);
                var da = new SqlDataAdapter(command);                
                var dt = new DataTable();
                await connection.OpenAsync();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    activities.Add(new ListingActivity
                    {
                        UserGuid = row["UserGuid"].ToString(),
                        VisitDate = Convert.ToDateTime(row["VisitDate"])
                    });
                }
            }
            return activities;
        }

        public async Task<IEnumerable<ListingActivity>> GetEnquiryListingActivitiesAsync(string query, int listingId)
        {
            var activities = new List<ListingActivity>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ListingID", listingId);
                var da = new SqlDataAdapter(command);
                var dt = new DataTable();
                await connection.OpenAsync();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    activities.Add(new ListingActivity
                    {
                        UserGuid = row["OwnerGuid"].ToString(),
                        VisitDate = Convert.ToDateTime(row["CreatedDate"])
                    });
                }
            }
            return activities;
        }


        public async Task<Profile> GetProfileByOwnerGuidAsync(string ownerGuid)
        {
            Profile profile = null;

            using (var connection = new SqlConnection(_connectionUser))
            {
                var command = new SqlCommand("SELECT * FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid", connection);
                command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                var da = new SqlDataAdapter(command);                
                var dt = new DataTable();
                await connection.OpenAsync();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    profile = new Profile
                    {
                        Name = row["Name"].ToString(),
                        ImageUrl = row["ImageUrl"].ToString()
                    };
                }
            }
            return profile;
        }

        public async Task<IList<ListingActivityVM>> GetListingActivityAsync(string ownerId, int activityType, bool isNotification = false)
        {
            var listing = await GetListingByOwnerIdAsync(ownerId);
            if (listing == null) return null;

            string activityText = string.Empty;
            IEnumerable<ListingActivity> listingActivities = null;

            if (activityType == Constantss.Like)
            {
                activityText = "Liked";
                listingActivities = await GetLikesByListingIdAsync(listing.Listingid);
            }
            else if (activityType == Constantss.Bookmark)
            {
                activityText = "Bookmarked";
                listingActivities = await GetBookmarksByListingIdAsync(listing.Listingid);
            }
            else if (activityType == Constantss.Subscribe)
            {
                activityText = "Subscribed";
                listingActivities = await GetSubscribersByListingIdAsync(listing.Listingid);
            }
            else if (activityType == Constantss.Enquiry)
            {
                activityText = "Enquiry";
                listingActivities = await GetEnquiryByListingIdAsync(listing.Listingid);
            }

            if (listingActivities == null) return null;

            var listingActivityVMs = listingActivities.Select(x => new ListingActivityVM
            {
                OwnerGuid = x.UserGuid,
                CompanyName = listing.CompanyName,
                VisitDate = x.VisitDate.ToString(Constantss.dateFormat1),
                ActivityType = activityType,
                ActivityText = activityText,
                isNotification = isNotification
            }).ToList();

            foreach (var activity in listingActivityVMs)
            {
                var profile = await GetProfileByOwnerGuidAsync(activity.OwnerGuid);

                if (profile != null)
                {
                    activity.UserName = profile.Name;
                    activity.ProfileImage = string.IsNullOrWhiteSpace(profile.ImageUrl) ? "resources/img/icon/profile.svg" : profile.ImageUrl;
                }
            }

            return listingActivityVMs;
        }

        public async Task<IList<ReviewListingViewModel>> GetReviewsByOwnerIdAsync(string ownerId)
        {
            var listing = await GetListingByOwnerIdAsync(ownerId);
            IList<ReviewListingViewModel> listReviews = new List<ReviewListingViewModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    // Use parameterized queries to avoid SQL injection and type issues
                    string query = @"SELECT r.RatingID, r.OwnerGuid, r.Comment, r.Date, r.Ratings, r.Time,
                     rr.Id AS RatingReplyId, rr.Message AS Reply, l.ListingID, l.CompanyName, l.ListingURL, l.BusinessCategory
                     FROM [listing].[Rating] r
                     JOIN [listing].[Listing] l ON r.ListingID = l.ListingID
                     LEFT JOIN [dbo].[RatingReply] rr ON r.RatingID = rr.RatingId
                     WHERE r.ListingID = @ListingID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ListingID", listing.Listingid); // Correctly pass the ListingID parameter

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var review = new ReviewListingViewModel
                                {
                                    RatingId = Convert.ToInt32(row["RatingID"]),
                                    OwnerGuid = row["OwnerGuid"].ToString(),
                                    Comment = row["Comment"].ToString(),
                                    Date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd hh:mm:ss tt"),
                                    Ratings = Convert.ToInt32(row["Ratings"]),
                                    VisitTime = row.IsNull("Time") ? string.Empty : Convert.ToDateTime(row["Time"]).ToString("HH:mm"),
                                    RatingReply = new RatingReply
                                    {
                                        Id = row.IsNull("RatingReplyId") ? 0 : Convert.ToInt32(row["RatingReplyId"]),
                                        Reply = row.IsNull("Reply") ? string.Empty : row["Reply"].ToString()
                                    },
                                    ListingId = Convert.ToInt32(row["ListingID"]),
                                    CompanyName = row["CompanyName"].ToString(),
                                    ListingUrl = row["ListingURL"].ToString(),
                                    BusinessCategory = row["BusinessCategory"].ToString()
                                };

                                review.UserName = await GetUserNameByOwnerGuidAsync(review.OwnerGuid);
                                review.UserImage = await GetUserImageByOwnerGuidAsync(review.OwnerGuid);
                                listReviews.Add(review);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception
                throw; // Re-throw or log exception based on your error handling strategy
            }

            return listReviews;
        }

        public async Task CreateOrUpdateReviewReply(RatingReply ratingReply)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = ratingReply.Id == 0 ?
                        @"INSERT INTO [dbo].[RatingReply] (RatingId, Message) VALUES (@RatingId, @Reply)" :
                        @"UPDATE [dbo].[RatingReply] SET Message = @Reply WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (ratingReply.Id != 0)
                        {
                            cmd.Parameters.AddWithValue("@Id", ratingReply.Id);
                        }
                        cmd.Parameters.AddWithValue("@RatingId", ratingReply.RatingId);
                        cmd.Parameters.AddWithValue("@Reply", ratingReply.Reply);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception
            }
        }

        private async Task<string> GetUserNameByOwnerGuidAsync(string ownerGuid)
        {
            string userName = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionUser))
                {
                    conn.Open();
                    string query = @"SELECT Name FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                        userName = (string)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception)
            {
                userName = "Unknown";
            }
            return userName;
        }

        private async Task<string> GetUserImageByOwnerGuidAsync(string ownerGuid)
        {
            string imageUrl = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionUser))
                {
                    conn.Open();
                    string query = @"SELECT ImageUrl FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                        imageUrl = (string)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception)
            {
                imageUrl = "resources/img/icon/profile.svg";
            }
            return imageUrl;
        }



        public async Task<IList<ListingActivityVM>> GetOwneridActivityAsync(string ownerId, int activityType, bool isNotification = false)
        {
            var listing = await GetListingByOwnerIdAsync(ownerId);
            if (listing == null) return null;

            string activityText = string.Empty;
            IEnumerable<ListingActivity> listingActivities = null;

            if (activityType == Constantss.Like)
            {
                activityText = "Liked";
                listingActivities = await GetLikesByOwnerIdAsync(ownerId);
            }
            else if (activityType == Constantss.Bookmark)
            {
                activityText = "Bookmarked";
                listingActivities = await GetBookmarksByOwnerIdAsync(ownerId);
            }
            else if (activityType == Constantss.Subscribe)
            {
                activityText = "Subscribed";
                listingActivities = await GetSubscribersByOwnerIdAsync(ownerId);
            }

            if (listingActivities == null) return null;

            var listingActivityVMs = listingActivities.Select(x => new ListingActivityVM
            {
                Listingid = x.Listingid,
                OwnerGuid = x.UserGuid,
                CompanyName = x.CompanyName,
                VisitDate = x.VisitDate.ToString(Constantss.dateFormat1),
                ActivityType = activityType,
                ActivityText = activityText,
                isNotification = isNotification
            }).ToList();

            foreach (var activity in listingActivityVMs)
            {
                var profile = await GetProfileByOwnerGuidAsync(activity.OwnerGuid);

                if (profile != null)
                {
                    activity.UserName = profile.Name;
                    activity.ProfileImage = string.IsNullOrWhiteSpace(profile.ImageUrl) ? "resources/img/icon/profile.svg" : profile.ImageUrl;
                }
            }

            return listingActivityVMs;
        }

        public async Task<IEnumerable<ListingActivity>> GetLikesByOwnerIdAsync(string OwnerId)
        {
            //return await GetOwnerActivitiesAsync("SELECT [LikeDislike].ListingID, [Listing].CompanyName,[LikeDislike].UserGuid,[LikeDislike].VisitDate FROM [AuditTrail].[audit].[LikeDislike] INNER JOIN [MimListing].[listing].[Listing] ON [AuditTrail].[audit].[LikeDislike].ListingID=[MimListing].[listing].[Listing].ListingID WHERE [AuditTrail].[audit].[LikeDislike].UserGuid = @UserGuid", OwnerId);
            return await GetOwnerActivitiesAsync("SELECT [LikeDislike].ListingID, [Listing].CompanyName,[LikeDislike].UserGuid,[LikeDislike].VisitDate FROM [AuditTrail_Api].[audit].[LikeDislike] INNER JOIN [MimListing_Api].[listing].[Listing] ON [AuditTrail_Api].[audit].[LikeDislike].ListingID=[MimListing_Api].[listing].[Listing].ListingID WHERE [AuditTrail_Api].[audit].[LikeDislike].UserGuid = @UserGuid", OwnerId);
        }

        public async Task<IEnumerable<ListingActivity>> GetBookmarksByOwnerIdAsync(string OwnerId)
        {
            //return await GetOwnerActivitiesAsync("SELECT [Bookmarks].ListingID, [Listing].CompanyName,[Bookmarks].UserGuid,[Bookmarks].VisitDate FROM [AuditTrail].[audit].[Bookmarks] INNER JOIN [MimListing].[listing].[Listing] ON [AuditTrail].[audit].[Bookmarks].ListingID=[MimListing].[listing].[Listing].ListingID WHERE [AuditTrail].[audit].[Bookmarks].UserGuid = @UserGuid", OwnerId);
            return await GetOwnerActivitiesAsync("SELECT [Bookmarks].ListingID, [Listing].CompanyName,[Bookmarks].UserGuid,[Bookmarks].VisitDate FROM [AuditTrail_Api].[audit].[Bookmarks] INNER JOIN [MimListing_Api].[listing].[Listing] ON [AuditTrail_Api].[audit].[Bookmarks].ListingID=[MimListing_Api].[listing].[Listing].ListingID WHERE [AuditTrail_Api].[audit].[Bookmarks].UserGuid = @UserGuid", OwnerId);
        }

        public async Task<IEnumerable<ListingActivity>> GetSubscribersByOwnerIdAsync(string OwnerId)
        {
            //return await GetOwnerActivitiesAsync("SELECT [Subscribes].ListingID, [Listing].CompanyName,[Subscribes].UserGuid,[Subscribes].VisitDate FROM [AuditTrail].[audit].[Subscribes] INNER JOIN [MimListing].[listing].[Listing] ON [AuditTrail].[audit].[Subscribes].ListingID=[MimListing].[listing].[Listing].ListingID WHERE [AuditTrail].[audit].[Subscribes].UserGuid = @UserGuid", OwnerId);
            return await GetOwnerActivitiesAsync("SELECT [Subscribes].ListingID, [Listing].CompanyName,[Subscribes].UserGuid,[Subscribes].VisitDate FROM [AuditTrail_Api].[audit].[Subscribes] INNER JOIN [MimListing_Api].[listing].[Listing] ON [AuditTrail_Api].[audit].[Subscribes].ListingID=[MimListing_Api].[listing].[Listing].ListingID WHERE [AuditTrail_Api].[audit].[Subscribes].UserGuid = @UserGuid", OwnerId);
        }

        public async Task<IEnumerable<ListingActivity>> GetOwnerActivitiesAsync(string query, string OwnerId)
        {
            var activities = new List<ListingActivity>();

            using (var connection = new SqlConnection(_connectionAudit))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserGuid", OwnerId);
                var da = new SqlDataAdapter(command);
                var dt = new DataTable();
                await connection.OpenAsync();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    activities.Add(new ListingActivity
                    {
                        Listingid = Convert.ToInt32(row["ListingID"]),
                        CompanyName = row["CompanyName"].ToString(),
                        UserGuid = row["UserGuid"].ToString(),
                        VisitDate = Convert.ToDateTime(row["VisitDate"])
                    });
                }
            }
            return activities;
        }

        public async Task<IList<ReviewListingViewModel>> GetReviewsByOwnerGuidIdAsync(string ownerId)
        {
            var listing = await GetListingByOwnerIdAsync(ownerId);
            IList<ReviewListingViewModel> listReviews = new List<ReviewListingViewModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    // Use parameterized queries to avoid SQL injection and type issues
                    string query = @"SELECT r.RatingID, r.OwnerGuid, r.Comment, r.Date, r.Ratings, r.Time,
                    rr.Id AS RatingReplyId, rr.Message AS Reply, l.ListingID, l.CompanyName, l.ListingURL, l.BusinessCategory
                    FROM [listing].[Rating] r
                    JOIN [listing].[Listing] l ON r.ListingID = l.ListingID
                    LEFT JOIN [dbo].[RatingReply] rr ON r.RatingID = rr.RatingId
                    WHERE r.OwnerGuid = @OwnerGuid";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerGuid", ownerId); // Correctly pass the ListingID parameter

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var review = new ReviewListingViewModel
                                {
                                    RatingId = Convert.ToInt32(row["RatingID"]),
                                    OwnerGuid = row["OwnerGuid"].ToString(),
                                    Comment = row["Comment"].ToString(),
                                    Date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd hh:mm:ss tt"),
                                    Ratings = Convert.ToInt32(row["Ratings"]),
                                    VisitTime = row.IsNull("Time") ? string.Empty : Convert.ToDateTime(row["Time"]).ToString("HH:mm"),
                                    RatingReply = new RatingReply
                                    {
                                        Id = row.IsNull("RatingReplyId") ? 0 : Convert.ToInt32(row["RatingReplyId"]),
                                        Reply = row.IsNull("Reply") ? string.Empty : row["Reply"].ToString()
                                    },
                                    ListingId = Convert.ToInt32(row["ListingID"]),
                                    CompanyName = row["CompanyName"].ToString(),
                                    ListingUrl = row["ListingURL"].ToString(),
                                    BusinessCategory = row["BusinessCategory"].ToString()
                                };

                                review.UserName = await GetUserNameByOwnerGuidAsync(review.OwnerGuid);
                                review.UserImage = await GetUserImageByOwnerGuidAsync(review.OwnerGuid);
                                listReviews.Add(review);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception
                throw; // Re-throw or log exception based on your error handling strategy
            }

            return listReviews;
        }
    }
}
