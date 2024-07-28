using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchListingsController : ControllerBase
    {
        private readonly SearchListingRepository _searchRepository;
        public SearchListingsController(SearchListingRepository searchListingRepository)
        {
            _searchRepository = searchListingRepository;
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<SearchHomeListingViewModel>>> SearchListings(string searchText)
        {
            var listings = await _searchRepository.GetApprovedListings();
            if (listings == null || !listings.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            var listingsWithAddress = listings.Where(x => x.Address != null).ToList();
            if (!listingsWithAddress.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            var filteredListings = listingsWithAddress.Where(x => x.CompanyName.ToLower().Contains(searchText.ToLower())).ToList();
            if (!filteredListings.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            var locationIds = filteredListings.Select(x => x.Address.AssemblyID).ToArray();
            var localities = await _searchRepository.GetLocalitiesByLocalityIds(locationIds);

            var result = filteredListings.Select(x => new SearchHomeListingViewModel
            {
                CompanyName = x.CompanyName,
                Id = x.ListingId,
                ListingUrl = x.ListingURL,
                CityName = localities.FirstOrDefault(l => l.Id == x.Address.AssemblyID)?.City.Name,
                LocalityName = localities.FirstOrDefault(l => l.Id == x.Address.AssemblyID)?.Name
            }).ToList();

            return Ok(result);
        }
    }
}
