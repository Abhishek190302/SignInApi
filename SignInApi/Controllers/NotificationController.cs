using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Reflection;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EnquiryListingRepository _enquiryListingRepository;
        private readonly string _connectionString;
        public NotificationController(UserService userService, IHttpContextAccessor httpContextAccessor, DashboardRepository dashboardRepository, EnquiryListingRepository enquiryListingRepository)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _dashboardRepository = dashboardRepository;
            _enquiryListingRepository = enquiryListingRepository;
        }


        [HttpGet]
        [Route("BailIconnotification")]
        public async Task<IActionResult> BailIconnotification()
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

                            // Get all activity types
                            var bookmarks = await _dashboardRepository.GetListingActivityAsync(currentUserGuid, Constantss.Bookmark);
                            var likes = await _dashboardRepository.GetListingActivityAsync(currentUserGuid, Constantss.Like);
                            var subscribes = await _dashboardRepository.GetListingActivityAsync(currentUserGuid, Constantss.Subscribe);
                            var enquiries = await _dashboardRepository.GetListingActivityAsync(currentUserGuid, Constantss.Enquiry);


                            // Combine all activities into a single list
                            var notifications = new List<ListingActivityVM>();
                            if (bookmarks != null) notifications.AddRange(bookmarks);
                            if (likes != null) notifications.AddRange(likes);
                            if (subscribes != null) notifications.AddRange(subscribes);
                            if (enquiries != null) notifications.AddRange(enquiries);

                            // Return combined notifications
                            var response = new
                            {
                                IsVendor = applicationUser.IsVendor,
                                Notification = notifications
                            };

                            return Ok(response);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { ErrorMessage = ex.Message });
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
    }
}
