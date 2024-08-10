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

        //[HttpGet]
        //[Route("search")]
        //public async Task<ActionResult<IEnumerable<SearchHomeListingViewModel>>> SearchListings(string searchText)
        //{
        //    var listings = await _searchRepository.GetApprovedListings();
        //    if (listings == null || !listings.Any())
        //        return Ok(new List<SearchHomeListingViewModel>());

        //    var listingsWithAddress = listings.Where(x => x.Address != null).ToList();
        //    if (!listingsWithAddress.Any())
        //        return Ok(new List<SearchHomeListingViewModel>());

        //    var filteredListings = listingsWithAddress.Where(async x => x.CompanyName.ToLower().Contains(searchText.ToLower()) || await _searchRepository.IsListingInCategory(x.Id, searchText.ToLower())).ToList();
        //    if (!filteredListings.Any())
        //        return Ok(new List<SearchHomeListingViewModel>());

        //    var locationIds = filteredListings.Select(x => x.Address.AssemblyID).ToArray();
        //    var localities = await _searchRepository.GetLocalitiesByLocalityIds(locationIds);

        //    var result = filteredListings.Select(x => new SearchHomeListingViewModel
        //    {
        //        CompanyName = x.CompanyName,
        //        listingId = x.ListingId,
        //        ListingUrl = x.ListingURL,
        //        CityName = localities.FirstOrDefault(l => l.Id == x.Address.AssemblyID)?.City.Name,
        //        LocalityName = localities.FirstOrDefault(l => l.Id == x.Address.AssemblyID)?.Name
        //    }).ToList();

        //    return Ok(result);
        //}

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SearchHomeListingViewModel>>> SearchListings(string searchText)
        {
            var listings = await _searchRepository.GetApprovedListings();
            if (listings == null || !listings.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            var listingsWithAddress = listings.Where(x => x.Address != null).ToList();
            if (!listingsWithAddress.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            var filteredListings = new List<ListingSearch>();
            foreach (var listing in listingsWithAddress)
            {
                if (listing.CompanyName.ToLower().Contains(searchText.ToLower()) ||
                    await _searchRepository.IsListingInCategory(int.Parse(listing.ListingId), searchText.ToLower()))
                {
                    filteredListings.Add(listing);
                }
            }

            if (!filteredListings.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            var locationIds = filteredListings.Select(x => x.Address.AssemblyID).ToArray();
            var localities = await _searchRepository.GetLocalitiesByLocalityIds(locationIds);

            var result = new List<SearchHomeListingViewModel>();
            foreach (var listing in filteredListings)
            {
                var subcategoryNames = await _searchRepository.GetSubcategoryNames(int.Parse(listing.ListingId));
                result.Add(new SearchHomeListingViewModel
                {
                    CompanyName = listing.CompanyName,
                    listingId = listing.ListingId,
                    ListingUrl = listing.ListingURL,
                    CityName = localities.FirstOrDefault(l => l.Id == listing.Address.AssemblyID)?.City.Name,
                    LocalityName = localities.FirstOrDefault(l => l.Id == listing.Address.AssemblyID)?.Name,
                    category = string.Join(", ", subcategoryNames)
                });
            }

            return Ok(result);
        }
    }
}
