using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Reflection;

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

        //[HttpGet("search")]
        //public async Task<ActionResult<IEnumerable<SearchHomeListingViewModel>>> SearchListings(string searchText)
        //{

        //    // Retrieve all approved listings
        //    var listings = await _searchRepository.GetApprovedListings();
        //    if (listings == null || !listings.Any())
        //        return Ok(new List<SearchHomeListingViewModel>());

        //    var listingsWithAddress = listings.Where(x => x.Address != null).ToList();
        //    if (!listingsWithAddress.Any())
        //        return Ok(new List<SearchHomeListingViewModel>());

        //    // Filter listings based on search text
        //    var filteredListings = new List<ListingSearch>();
        //    string matchingCategoryName = string.Empty;
        //    int dynamicCategoryId = 0;

        //    foreach (var listing in listingsWithAddress)
        //    {
        //        if (listing.CompanyName.ToLower().Contains(searchText.ToLower()) ||
        //            listing.Keyword.ToLower().Contains(searchText.ToLower()) || // Match on keyword
        //            await _searchRepository.IsListingInCategory(int.Parse(listing.ListingId), searchText.ToLower()) ||
        //            await _searchRepository.IsCategoryMatching(searchText.ToLower()))
        //        {
        //            // Store the exact category name and ID that matched
        //            var matchedCategory = await _searchRepository.GetCategoryByName(searchText.ToLower());
        //            if (matchedCategory != null)
        //            {
        //                matchingCategoryName = matchedCategory.Name;
        //                dynamicCategoryId = matchedCategory.Id;
        //            }

        //            filteredListings.Add(listing);
        //        }
        //    }

        //    if (!filteredListings.Any())
        //        return Ok(new List<SearchHomeListingViewModel>());

        //    // Retrieve location details
        //    var locationIds = filteredListings.Select(x => x.Address.AssemblyID).ToArray();
        //    var localities = await _searchRepository.GetLocalitiesByLocalityIds(locationIds);

        //    var result = new List<SearchHomeListingViewModel>();

        //    // Add the filtered listings to the result
        //    foreach (var listing in filteredListings)
        //    {
        //        var subCategories = await _searchRepository.GetSubcategory(int.Parse(listing.ListingId));
        //        var subcategoryNames = subCategories.Select(sub => sub.Name).ToList();

        //        var city = localities.FirstOrDefault(l => l.Id == listing.Address.AssemblyID)?.City.Name;
        //        var locality = localities.FirstOrDefault(l => l.Id == listing.Address.AssemblyID)?.Name;

        //        result.Add(new SearchHomeListingViewModel
        //        {
        //            CompanyName = listing.CompanyName,
        //            listingId = listing.ListingId,
        //            ListingUrl = listing.ListingURL,
        //            CityName = city,
        //            LocalityName = locality,
        //            category = string.Join(", ", subcategoryNames),
        //            CategoryId = subCategories.FirstOrDefault()?.Id ?? 0,
        //            keyword = listing.Keyword,  // Include the keyword in the result
        //            keywordId = listing.ListingId
        //        });
        //    }

        //    // Add a separate result that displays the matching category name with dynamic category ID
        //    if (!string.IsNullOrEmpty(matchingCategoryName))
        //    {
        //        result.Insert(0, new SearchHomeListingViewModel
        //        {
        //            CompanyName = null,
        //            listingId = null,
        //            ListingUrl = null,
        //            CityName = null,
        //            LocalityName = null,
        //            category = matchingCategoryName,
        //            CategoryId = dynamicCategoryId // Pass the dynamic category ID
        //        });
        //    }

        //    return Ok(result);
        //}

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SearchHomeListingViewModel>>> SearchListings(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return Ok(new List<SearchHomeListingViewModel>());

            // Step 1: Retrieve approved listings and filter by non-null addresses
            var listings = await _searchRepository.GetApprovedListings();
            if (listings == null || !listings.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            var listingsWithAddress = listings.Where(x => x.Address != null).ToList();
            if (!listingsWithAddress.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            // Step 2: Prepare to match listings based on search text
            searchText = searchText.ToLower();
            var filteredListings = new List<ListingSearch>();
            string matchingCategoryName = string.Empty;
            int dynamicCategoryId = 0;

            foreach (var listing in listingsWithAddress)
            {
                bool isMatched = false;

                // Check if the CompanyName or Keyword contains the search text
                if (listing.CompanyName.ToLower().Contains(searchText) ||
                    listing.Keyword.ToLower().Contains(searchText))
                {
                    isMatched = true;
                }

                // Check if listing is in a category matching the search text
                if (!isMatched && await _searchRepository.IsListingInCategory(int.Parse(listing.ListingId), searchText))
                {
                    isMatched = true;
                }

                // Check if any category matches the search text
                if (!isMatched)
                {
                    var matchedCategory = await _searchRepository.GetCategoryByName(searchText);
                    if (matchedCategory != null)
                    {
                        matchingCategoryName = matchedCategory.Name;
                        dynamicCategoryId = matchedCategory.Id;
                        isMatched = true;
                    }
                }

                // Add matched listing to filtered listings
                if (isMatched)
                {
                    filteredListings.Add(listing);
                }
            }

            if (!filteredListings.Any())
                return Ok(new List<SearchHomeListingViewModel>());

            // Step 3: Retrieve locality details based on matched listings
            var locationIds = filteredListings.Select(x => x.Address.AssemblyID).ToArray();
            var localities = await _searchRepository.GetLocalitiesByLocalityIds(locationIds);

            var result = new List<SearchHomeListingViewModel>();

            // Step 4: Create the result list with filtered listings
            foreach (var listing in filteredListings)
            {
                // Get subcategories for each listing
                var subCategories = await _searchRepository.GetSubcategory(int.Parse(listing.ListingId));
                var subcategoryNames = subCategories.Select(sub => sub.Name).ToList();

                // Retrieve city and locality names
                var localityDetails = localities.FirstOrDefault(l => l.Id == listing.Address.AssemblyID);
                var city = localityDetails?.City.Name;
                var locality = localityDetails?.Name;

                result.Add(new SearchHomeListingViewModel
                {
                    CompanyName = listing.CompanyName,
                    listingId = listing.ListingId,
                    ListingUrl = listing.ListingURL,
                    CityName = city,
                    LocalityName = locality,
                    category = string.Join(", ", subcategoryNames),
                    CategoryId = subCategories.FirstOrDefault()?.Id ?? 0,
                    keyword = listing.Keyword,
                    keywordId = listing.ListingId
                });
            }

            // Step 5: Add the dynamic category result if a category name matched
            if (!string.IsNullOrEmpty(matchingCategoryName))
            {
                result.Insert(0, new SearchHomeListingViewModel
                {
                    CompanyName = null,
                    listingId = null,
                    ListingUrl = null,
                    CityName = null,
                    LocalityName = null,
                    category = matchingCategoryName,
                    CategoryId = dynamicCategoryId
                });
            }

            return Ok(result);
        }
    }
}
