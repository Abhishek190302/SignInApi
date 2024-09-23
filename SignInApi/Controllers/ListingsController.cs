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
        public async Task<IActionResult> GetCategoriesListing(int pageNumber = 1, int pageSize = 10 ,int subCategoryid = 94, string cityName = null)
        {   
            try
            {
                var listings = await _listingService.GetListings(pageNumber, pageSize, subCategoryid, cityName);
                return Ok(listings);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting categories listing");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        [Route("GetCategoriesListingid")]
        public async Task<IActionResult> GetCategoriesListingid(int subCategoryid = 94, string cityName = null, int liistingId = 10881)
        {
            try
            {
                var listings = await _listingService.GetListingsid(subCategoryid, cityName, liistingId);
                return Ok(listings);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting categories listing");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
