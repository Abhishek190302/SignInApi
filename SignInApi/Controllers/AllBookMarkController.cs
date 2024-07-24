using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Security.Claims;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllBookMarkController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        public AllBookMarkController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("AuditTrail");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetUserAllMyBookmarks")]
        public async Task<IActionResult> GetUserAllMyBookmarks(int activityType, bool isNotification = false)
        {
            try
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
                            var listingActivityVMs = await GetListingActivityAsync(currentUserGuid, activityType, isNotification);
                            if (listingActivityVMs == null)
                            {
                                return NotFound();
                            }

                            var response = new
                            {
                                IsVendor = applicationUser.IsVendor,
                                Bookmarks = listingActivityVMs
                            };

                            return Ok(response);




                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                return Ok();



                //var applicationUser = await GetUserByUserNameAsync(userName);
                //if (applicationUser == null)
                //{
                //    return NotFound("User not found.");
                //}


            }
            catch (Exception exc)
            {
                return StatusCode(500, new { ErrorMessage = exc.Message });
            }
        }

        private async Task<IList<ListingActivityVM>> GetListingActivityAsync(string ownerId, int activityType, bool isNotification)
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

        private async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            ApplicationUser user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Users WHERE UserName = @UserName", connection);
                command.Parameters.AddWithValue("@UserName", userName);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new ApplicationUser
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            IsVendor = reader.GetBoolean(reader.GetOrdinal("IsVendor"))
                        };
                    }
                }
            }
            return user;
        }

        private async Task<Listing> GetListingByOwnerIdAsync(string ownerId)
        {
            Listing listing = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Listings WHERE OwnerId = @OwnerId", connection);
                command.Parameters.AddWithValue("@OwnerId", ownerId);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        listing = new Listing
                        {
                            Listingid = reader.GetInt32(reader.GetOrdinal("ListingID")),
                            CompanyName = reader.GetString(reader.GetOrdinal("CompanyName"))
                        };
                    }
                }
            }
            return listing;
        }

        private async Task<IEnumerable<ListingActivity>> GetLikesByListingIdAsync(int listingId)
        {
            return await GetListingActivitiesAsync("SELECT * FROM Likes WHERE ListingID = @ListingID", listingId);
        }

        private async Task<IEnumerable<ListingActivity>> GetBookmarksByListingIdAsync(int listingId)
        {
            return await GetListingActivitiesAsync("SELECT * FROM Bookmarks WHERE ListingID = @ListingID", listingId);
        }

        private async Task<IEnumerable<ListingActivity>> GetSubscribersByListingIdAsync(int listingId)
        {
            return await GetListingActivitiesAsync("SELECT * FROM Subscribers WHERE ListingID = @ListingID", listingId);
        }

        private async Task<IEnumerable<ListingActivity>> GetListingActivitiesAsync(string query, int listingId)
        {
            var activities = new List<ListingActivity>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ListingID", listingId);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        activities.Add(new ListingActivity
                        {
                            UserGuid = reader.GetString(reader.GetOrdinal("UserGuid")),
                            VisitDate = reader.GetDateTime(reader.GetOrdinal("VisitDate"))
                        });
                    }
                }
            }
            return activities;
        }

        private async Task<Profile> GetProfileByOwnerGuidAsync(string ownerGuid)
        {
            Profile profile = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Profiles WHERE OwnerGuid = @OwnerGuid", connection);
                command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        profile = new Profile
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                        };
                    }
                }
            }
            return profile;
        }
    }
}
