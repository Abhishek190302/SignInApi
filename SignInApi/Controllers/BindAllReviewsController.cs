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
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public BindAllReviewsController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor, CompanyDetailsRepository companydetailsRepository)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
        }

        [HttpPost]
        [Route("UserReviews")]
        public async Task<ActionResult> UserReviews(ReviewsVM reviewsVM)
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
                            var reviews = await GetReviewsAsync(reviewsVM.companyID);
                            return Ok(reviews);
                        }
                        else
                        {
                            var reviews = await GetReviewsAsync(reviewsVM.companyID);
                            return Ok(reviews);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User Not Found");
            }
            return Unauthorized();

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
    }
}
