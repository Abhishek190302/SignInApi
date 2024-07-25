using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyActivityController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        public MyActivityController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor, DashboardRepository dashboardRepository)
        {
            _connectionString = configuration.GetConnectionString("AuditTrail");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        [Route("MyActivityAllMyBookmarks")]
        public async Task<IActionResult> MyActivityAllMyBookmarks()
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
                            var listingActivityVMs = await _dashboardRepository.GetOwneridActivityAsync(currentUserGuid, Constantss.Bookmark);
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
                    return NotFound("User Not Found");
                }
                return Unauthorized();
            }
            catch (Exception exc)
            {
                return StatusCode(500, new { ErrorMessage = exc.Message });
            }
        }

        [HttpGet]
        [Route("MyActivityAllMyLikes")]
        public async Task<IActionResult> GetUserAllMyLikes()
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
                            var listingActivityVMs = await _dashboardRepository.GetOwneridActivityAsync(currentUserGuid, Constantss.Like);
                            if (listingActivityVMs == null)
                            {
                                return NotFound();
                            }

                            var response = new
                            {
                                IsVendor = applicationUser.IsVendor,
                                LikeDislike = listingActivityVMs
                            };

                            return Ok(response);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    return NotFound("User Not Found");
                }
                return Unauthorized();
            }
            catch (Exception exc)
            {
                return StatusCode(500, new { ErrorMessage = exc.Message });
            }
        }

        [HttpGet]
        [Route("MyActivityAllMySubscribe")]
        public async Task<IActionResult> GetUserAllMySubscribe()
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
                            var listingActivityVMs = await _dashboardRepository.GetOwneridActivityAsync(currentUserGuid, Constantss.Subscribe);
                            if (listingActivityVMs == null)
                            {
                                return NotFound();
                            }

                            var response = new
                            {
                                IsVendor = applicationUser.IsVendor,
                                Subscribes = listingActivityVMs
                            };

                            return Ok(response);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    return NotFound("User Not Found");
                }
                return Unauthorized();
            }
            catch (Exception exc)
            {
                return StatusCode(500, new { ErrorMessage = exc.Message });
            }
        }

        [HttpPost]
        [Route("MyActivityAllMyReviews")]
        public async Task<IActionResult> MyActivityAllMyReviews([FromBody] ReviewRequest reviewRequest)
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
                            if (reviewRequest.Operation == "GetReviews")
                            {
                                var reviews = await _dashboardRepository.GetReviewsByOwnerGuidIdAsync(currentUserGuid);
                                return Ok(reviews);
                            }
                            else if (reviewRequest.Operation == "CreateReviewReply" || reviewRequest.Operation == "UpdateReviewReply")
                            {
                                await _dashboardRepository.CreateOrUpdateReviewReply(reviewRequest.RatingReply);
                                return Ok();
                            }
                            else
                            {
                                return BadRequest("Invalid operation");
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    return NotFound("User Not Found");
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
