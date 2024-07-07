using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IListingService _listingService;
        private readonly ILogger<ListingService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ListingsController(IListingService listingService, ILogger<ListingService> logger, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _listingService = listingService;
            _logger = logger;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet] 
        [Route("GetCategoriesListing")]
        public async Task<IActionResult> GetCategoriesListing()
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
                        var listings = await _listingService.GetListings();
                        return Ok(listings);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting categories listing");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
