using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsEnquiryController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EnquiryListingRepository _enquiryListingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ListingsEnquiryController(UserService userService, IHttpContextAccessor httpContextAccessor, EnquiryListingRepository enquiryListingRepository)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _enquiryListingRepository = enquiryListingRepository;

        }

        [HttpGet]
        [Route("GetEnquiries")]
        public async Task<IActionResult> GetEnquiries()
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
                            var listing = await _enquiryListingRepository.GetListingByOwnerIdAsync(currentUserGuid);
                            if (listing == null)
                            {
                                return NotFound("Listing not found");
                            }

                            var enquiries = await _enquiryListingRepository.GetEnquiryByListingIdAsync(listing.Listingid);
                            return Ok(enquiries);
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
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
