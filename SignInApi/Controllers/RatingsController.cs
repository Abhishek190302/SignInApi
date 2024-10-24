using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        private readonly IConfiguration _configuration;
        public RatingsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, UserService userService, CompanyDetailsRepository companyDetailsRepository)
        {
            _configuration = configuration;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companyDetailsRepository;
        }

        [HttpPost]
        [Route("CreateOrUpdateRating")]
        public async Task<IActionResult> CreateOrUpdateRating([FromBody] RatingDetail ratingDetailVM)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        if (listing != null)
                        {
                            if (ratingDetailVM.Ratings <= 0 || string.IsNullOrEmpty(ratingDetailVM.Comment))
                            {
                                return BadRequest(new { message = "Rating & Comment Required." });
                            }

                            DateTime timeZoneDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            string time = timeZoneDate.ToString("hh:mm tt");
                            DateTime currentTime = DateTime.Parse(time, System.Globalization.CultureInfo.CurrentCulture);

                            try
                            {
                                var userProfile = await GetProfileByOwnerGuidAsync(currentUserGuid);
                                var rating = await GetRatingByListingIdAndOwnerId(listing.Listingid, currentUserGuid);
                                bool recordNotFound = rating == null;

                                if (recordNotFound)
                                {
                                    rating = new Rating
                                    {
                                        ListingID = listing.Listingid,
                                        OwnerGuid = currentUserGuid,
                                        IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                        Date = currentTime,
                                        Time = currentTime,
                                        Ratings = ratingDetailVM.Ratings,
                                        Comment = ratingDetailVM.Comment,
                                        ImageUrl = userProfile.ImageUrl
                                    };

                                    await AddRatingAsync(rating);
                                }
                                else
                                {
                                    rating.Ratings = ratingDetailVM.Ratings;
                                    rating.Comment = ratingDetailVM.Comment;
                                    await UpdateRatingAsync(rating);
                                }

                                var listReviews = await GetReviewsAsync(listing.Listingid);
                                return Ok(new { message = "Thank you for submitting your review.", reviews = listReviews });
                            }
                            catch (Exception exc)
                            {
                                return StatusCode(500, new { message = exc.Message + exc.InnerException?.ToString() });
                            }

                        }
                        else
                        {

                            DateTime timeZoneDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            string time = timeZoneDate.ToString("hh:mm tt");
                            DateTime currentTime = DateTime.Parse(time, System.Globalization.CultureInfo.CurrentCulture);
                            var userProfile = await GetProfileByOwnerGuidAsync(currentUserGuid);
                            var rating = await GetRatingByListingIdAndOwnerId(ratingDetailVM.companyID, currentUserGuid);
                            bool recordNotFound = rating == null;

                            if (recordNotFound)
                            {
                                rating = new Rating
                                {
                                    ListingID = ratingDetailVM.companyID,
                                    OwnerGuid = currentUserGuid,
                                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                    Date = currentTime,
                                    Time = currentTime,
                                    Ratings = ratingDetailVM.Ratings,
                                    Comment = ratingDetailVM.Comment,
                                    ImageUrl = userProfile.ImageUrl
                                };

                                await AddRatingAsync(rating);
                            }
                            else
                            {
                                rating.Ratings = ratingDetailVM.Ratings;
                                rating.Comment = ratingDetailVM.Comment;
                                await UpdateRatingAsync(rating);
                            }

                            var listReviews = await GetReviewsAsync(ratingDetailVM.companyID);
                            return Ok(new { message = "Thank you for submitting your review.", reviews = listReviews });
                        }
                    }
                    catch (Exception EX)
                    {
                        throw;
                    }
                }
                return NotFound("User Not Found");
            }
            return Unauthorized();
        }

        private async Task<Rating> GetRatingByListingIdAndOwnerId(int listingId, string ownerGuid)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MimListing")))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Rating] WHERE ListingID = @ListingID AND OwnerGuid = @OwnerGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                cmd.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Rating
                    {
                        ListingID = (int)row["ListingID"],
                        OwnerGuid = (string)row["OwnerGuid"],
                        IPAddress = row["IPAddress"].ToString(),
                        Date = (DateTime)row["Date"],
                        Time = (DateTime)row["Time"],
                        Ratings = (int)row["Ratings"],
                        Comment = row["Comment"].ToString()
                    };
                }
            }
            return null;
        }

        private async Task AddRatingAsync(Rating rating)
        {
            bool approvedbyAdmin = false;

            // Implement the method to add rating using ADO.NET
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MimListing")))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO [listing].[Rating] (ListingID, OwnerGuid, IPAddress, Date, Time, Ratings, Comment, ApprovedByAdmin) VALUES (@ListingID, @OwnerGuid, @IPAddress, @Date, @Time, @Ratings, @Comment, @ApprovedByAdmin)", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", rating.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", rating.OwnerGuid);
                    cmd.Parameters.AddWithValue("@IPAddress", rating.IPAddress);
                    cmd.Parameters.AddWithValue("@Date", rating.Date);
                    cmd.Parameters.AddWithValue("@Time", rating.Time);
                    cmd.Parameters.AddWithValue("@Ratings", rating.Ratings);
                    cmd.Parameters.AddWithValue("@Comment", rating.Comment);
                    cmd.Parameters.AddWithValue("@ApprovedByAdmin", approvedbyAdmin);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UpdateRatingAsync(Rating rating)
        {
            // Implement the method to update rating using ADO.NET
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MimListing")))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("UPDATE [listing].[Rating] SET Ratings = @Ratings, Comment = @Comment WHERE ListingID = @ListingID AND OwnerGuid = @OwnerGuid", conn))
                {
                    cmd.Parameters.AddWithValue("@Ratings", rating.Ratings);
                    cmd.Parameters.AddWithValue("@Comment", rating.Comment);
                    cmd.Parameters.AddWithValue("@ListingID", rating.ListingID);
                    cmd.Parameters.AddWithValue("@OwnerGuid", rating.OwnerGuid);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        //private async Task<List<Rating>> GetReviewsAsync(int listingId,string imageUrl)
        //{
            
        //    var reviews = new List<Rating>();
        //    using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MimListing")))
        //    {
        //        await conn.OpenAsync();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Rating] WHERE ListingID = @ListingID", conn);
        //        cmd.Parameters.AddWithValue("@ListingID", listingId);
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            reviews.Add(new Rating
        //            {
        //                ListingID = (int)row["ListingID"],
        //                OwnerGuid = (string)row["OwnerGuid"],
        //                IPAddress = row["IPAddress"].ToString(),
        //                Date = (DateTime)row["Date"],
        //                Time = (DateTime)row["Time"],
        //                Ratings = (int)row["Ratings"],
        //                Comment = row["Comment"].ToString(),
        //                ImageUrl = imageUrl
        //            });
        //        }
        //    }
        //    return reviews;
        //}



        private async Task<List<Rating>> GetReviewsAsync(int listingId)
        {
            try
            {
                var reviews = new List<Rating>();

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MimListing")))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(@"
                    SELECT r.ListingID, r.OwnerGuid, r.IPAddress, r.Date, r.Time, r.Ratings, r.Comment, up.ImageUrl
                    FROM [listing].[Rating] r
                    LEFT JOIN [MimUser_Api].[dbo].[UserProfile] up ON r.OwnerGuid = up.OwnerGuid
                    WHERE r.ListingID = @ListingID", conn);
                    cmd.Parameters.AddWithValue("@ListingID", listingId);

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
            catch(Exception ex)
            {
                throw;
            }
            
        }

        private async Task<UserProfileRequest> GetProfileByOwnerGuidAsync(string ownerGuid)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("MimUser")))
            {
                var command = new SqlCommand("SELECT Name, LastName, ImageUrl, Gender FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid", conn);
                command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                var dt = new DataTable();
                var da = new SqlDataAdapter(command);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    return new UserProfileRequest
                    {
                        Name = row["Name"].ToString(),
                        LastName = row["LastName"].ToString(),
                        ImageUrl = row["ImageUrl"].ToString(),
                        Gender = row["Gender"].ToString()
                    };
                }

                return null;

            }
        }
    }
}
