using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindAllReviewsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _connectionUserString;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public BindAllReviewsController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor, CompanyDetailsRepository companydetailsRepository)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionUserString = configuration.GetConnectionString("MimUser");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
        }

        //[HttpPost]
        //[Route("UserReviews")]
        //public async Task<ActionResult> UserReviews(ReviewsVM reviewsVM)
        //{
        //    var user = _httpContextAccessor.HttpContext.User;

        //    if (user.Identity.IsAuthenticated)
        //    {
        //        var userName = user.Identity.Name;

        //        var applicationUser = await _userService.GetUserByUserName(userName);
        //        if (applicationUser != null)
        //        {
        //            try
        //            {
        //                string currentUserGuid = applicationUser.Id.ToString();
        //                var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
        //                if (listing != null)
        //                {
        //                    var reviews = await GetReviewsAsync(reviewsVM.companyID);
        //                    return Ok(reviews);
        //                }
        //                else
        //                {
        //                    var reviews = await GetReviewsAsync(reviewsVM.companyID);
        //                    return Ok(reviews);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw;
        //            }
        //        }
        //        return NotFound("User Not Found");
        //    }
        //    return Unauthorized();

        //}


        [HttpPost]
        [Route("UserReviews")]
        public async Task<ActionResult> UserReviews(ReviewsVM reviewsVM)
        {
            var reviews = await GetReviewsAsync(reviewsVM.companyID);
            var totalReviewCount = reviews.Count();
            var ratingSum = reviews.Sum(r => r.Ratings); // Sum of all ratings
            var averageRating = totalReviewCount > 0 ? (double)ratingSum / totalReviewCount : 0; // Calculate average rating
            //return Ok(reviews);
            return Ok(new {reviews = reviews, reviewCount = totalReviewCount, averageRating = averageRating });
        }

        [HttpPost]
        [Route("GetUserAllReviews")]
        public async Task<IActionResult> GetUserAllReviews([FromBody] ReviewRequestVM reviewRequestVM)
        {
            
            if (reviewRequestVM.Operation == "GetReviews")
            {
                var reviews = await GetReviewsByOwnerIdAsync(reviewRequestVM.companyID);
                return Ok(reviews);
            }
            else if (reviewRequestVM.Operation == "CreateReviewReply" || reviewRequestVM.Operation == "UpdateReviewReply")
            {
                await CreateOrUpdateReviewReply(reviewRequestVM.RatingReply);
                return Ok();
            }
            else
            {
                return BadRequest("Invalid operation");
            }         
        }

        private async Task CreateOrUpdateReviewReply(RatingReply ratingReply)
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

        private async Task<List<Rating>> GetReviewsAsync(int listingId)
        {
            try
            {
                var reviews = new List<Rating>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(@"
                    SELECT r.ListingID, r.OwnerGuid, r.IPAddress, r.Date, r.Time, r.Ratings, r.Comment, up.ImageUrl
                    FROM [listing].[Rating] r
                    LEFT JOIN [MimUser_Api].[dbo].[UserProfile] up ON r.OwnerGuid = up.OwnerGuid
                    WHERE r.ListingID = @ListingID", conn);
                    cmd.Parameters.AddWithValue("@ListingID", listingId);


                    //SqlCommand cmd = new SqlCommand(@"
                    //SELECT r.ListingID, r.OwnerGuid, r.IPAddress, r.Date, r.Time, r.Ratings, r.Comment, up.ImageUrl
                    //FROM [listing].[Rating] r
                    //LEFT JOIN [MimUsers].[dbo].[UserProfile] up ON r.OwnerGuid = up.OwnerGuid
                    //WHERE r.ListingID = @ListingID", conn);
                    //cmd.Parameters.AddWithValue("@ListingID", listingId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        reviews.Add(new Rating
                        {
                            ListingID = (int)row["ListingID"],
                            OwnerGuid = (string)row["OwnerGuid"],
                            IPAddress = row["IPAddress"].ToString(),
                            Date = (DateTime)row["Date"],
                            Time = (DateTime)row["Time"],
                            Ratings = (int)row["Ratings"],
                            Comment = row["Comment"].ToString(),
                            ImageUrl = row["ImageUrl"] == DBNull.Value ? null : row["ImageUrl"].ToString()
                        });
                    }
                }

                return reviews;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<IList<ReviewListingViewModel>> GetReviewsByOwnerIdAsync(int listingid)
        {
            var listing = await GetListingByOwnerIdAsync(listingid);
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
                                var reviewDate = Convert.ToDateTime(row["Date"]);
                                var timeAgo = GetTimeAgo(reviewDate); // Use a helper function to calculate the time ago
                                
                                var review = new ReviewListingViewModel
                                {
                                    RatingId = Convert.ToInt32(row["RatingID"]),
                                    OwnerGuid = row["OwnerGuid"].ToString(),
                                    Comment = row["Comment"].ToString(),
                                    //Date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd hh:mm:ss tt"),
                                    Date = reviewDate.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                    TimeAgo = timeAgo,
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

                                //review.UserName = await GetUserNameByOwnerGuidAsync(review.OwnerGuid);
                                var (userName, gender) = await GetUserNameAndGenderByOwnerGuidAsync(review.OwnerGuid);
                                review.UserName = userName;
                                review.Gender = gender;
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

        private string GetTimeAgo(DateTime reviewDate)
        {
            TimeSpan timeDifference = DateTime.Now - reviewDate;

            if (timeDifference.TotalDays < 1)
            {
                if (timeDifference.TotalHours < 1)
                    return $"{Math.Floor(timeDifference.TotalMinutes)} minutes ago";
                else
                    return $"{Math.Floor(timeDifference.TotalHours)} hours ago";
            }
            else if (timeDifference.TotalDays < 7)
            {
                return $"{Math.Floor(timeDifference.TotalDays)} days ago";
            }
            else if (timeDifference.TotalDays < 30)
            {
                return $"{Math.Floor(timeDifference.TotalDays / 7)} weeks ago";
            }
            else if (timeDifference.TotalDays < 365)
            {
                return $"{Math.Floor(timeDifference.TotalDays / 30)} months ago";
            }
            else
            {
                return $"{Math.Floor(timeDifference.TotalDays / 365)} years ago";
            }
        }

        private async Task<Listing> GetListingByOwnerIdAsync(int listingid)
        {
            Listing listing = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE ListingID = @ListingID", connection);
                command.Parameters.AddWithValue("@ListingID", listingid);
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

        //private async Task<string> GetUserNameByOwnerGuidAsync(string ownerGuid)
        //{
        //    string userName = string.Empty;
        //    string Gender = string.Empty;
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(_connectionUserString))
        //        {
        //            conn.Open();
        //            string query = @"SELECT Name,Gender FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid";

        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
        //                userName = (string)await cmd.ExecuteScalarAsync();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        userName = "Unknown";
        //    }
        //    return userName;
        //}

        private async Task<(string UserName, string Gender)> GetUserNameAndGenderByOwnerGuidAsync(string ownerGuid)
        {
            string userName = string.Empty;
            string gender = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionUserString))
                {
                    await conn.OpenAsync();
                    string query = @"SELECT Name, Gender FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerGuid", ownerGuid);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                userName = reader["Name"].ToString();
                                gender = reader["Gender"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                userName = "Unknown";
                gender = "Unknown";
            }

            return (userName, gender);
        }

        private async Task<string> GetUserImageByOwnerGuidAsync(string ownerGuid)
        {
            string imageUrl = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionUserString))
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
    }
}
