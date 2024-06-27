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
        private readonly IListingService _listingService;
        private readonly ILogger<ListingService> _logger;
        public ListingsController(IListingService listingService, ILogger<ListingService> logger)
        {
            _listingService = listingService;
            _logger = logger;
        }

        [HttpGet] 
        [Route("GetCategoriesListing")]
        public async Task<IActionResult> GetCategoriesListing()
        {   
            try
            {
                var listings = await _listingService.GetListings();
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
