using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingActivityDashboardController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        public ListingActivityDashboardController(UserService userService, IHttpContextAccessor httpContextAccessor, DashboardRepository dashboardRepository)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        [Route("GetListingActivityDashboardCounts")]
        public async Task<ActionResult<ListingActivityCount>> GetListingActivityDashboardCounts()
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
                        var listing = await _dashboardRepository.GetListingByOwnerId(currentUserGuid);
                        if (listing != null)
                        {
                            var LikesCount = await _dashboardRepository.GetLikesByListingId(listing.Listingid);
                            var BookmarksCount = await _dashboardRepository.GetBookmarksByListingId(listing.Listingid);
                            var SubscribersCount = await _dashboardRepository.GetSubscribersByListingId(listing.Listingid);
                            var ReviewsCount = await _dashboardRepository.GetRatingsByListingId(listing.Listingid);

                            return new ListingActivityCount
                            {
                                BookmarksCount = BookmarksCount,
                                LikesCount = LikesCount,
                                SubscribersCount = SubscribersCount,
                                ReviewsCount = ReviewsCount
                            };
                        }
                        else
                        {
                            return new ListingActivityCount
                            {
                                BookmarksCount = 0,
                                LikesCount = 0,
                                SubscribersCount = 0,
                                ReviewsCount = 0
                            };
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }


        [HttpGet]
        [Route("GetListingMyActivityCounts")]
        public async Task<ActionResult<ListingActivityCount>> GetListingMyActivityCounts()
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
                        //var listing = await _dashboardRepository.GetListingByOwnerId(currentUserGuid);
                        
                            var LikesCount = await _dashboardRepository.GetLikesByOwnerId(currentUserGuid);
                            var BookmarksCount = await _dashboardRepository.GetBookmarksByOwnerId(currentUserGuid);
                            var SubscribersCount = await _dashboardRepository.GetSubscribersByOwnerId(currentUserGuid);
                            var ReviewsCount = await _dashboardRepository.GetRatingsByOwnerId(currentUserGuid);

                            return new ListingActivityCount
                            {
                                BookmarksCount = BookmarksCount,
                                LikesCount = LikesCount,
                                SubscribersCount = SubscribersCount,
                                ReviewsCount = ReviewsCount
                            };
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }
    }
}
