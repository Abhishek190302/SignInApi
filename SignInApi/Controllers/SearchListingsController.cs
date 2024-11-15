using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchListingsController : ControllerBase
    {
        private readonly SearchListingRepository _searchRepository;
        private readonly IConfiguration _configuration;
        public SearchListingsController(SearchListingRepository searchListingRepository, IConfiguration configuration)
        {
            _searchRepository = searchListingRepository;
            _configuration = configuration;

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

        //[HttpGet("search")]
        //public async Task<ActionResult<IEnumerable<SearchHomeListingViewModel>>> SearchListings(string searchText)
        //{
        //    if (string.IsNullOrWhiteSpace(searchText))
        //        return Ok(new List<SearchHomeListingViewModel>());

        //    // Define variables outside of any loops or conditions
        //    string matchingCategoryName = string.Empty;
        //    int dynamicCategoryId = 0;

        //    // Check if the search text is a valid mobile number
        //    bool isMobileNumber = long.TryParse(searchText, out _) && searchText.Length >= 1;
        //    bool isGstNumber = Regex.IsMatch(searchText, @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$"); // Assuming GST is 15 alphanumeric characters
        //    List<ListingSearch> listings;

        //    if (isMobileNumber)
        //    {
        //        // Retrieve listings by mobile number
        //        listings = await GetListingsByMobileNumber(searchText);
        //    }
        //    else if (isGstNumber)
        //    {
        //        // Retrieve listings by GST number
        //        listings = await GetListingsByGstNumber(searchText);
        //    }
        //    else
        //    {
        //        // Step 1: Retrieve approved listings and filter by non-null addresses
        //         listings = (await _searchRepository.GetApprovedListings()).ToList();
        //        if (listings == null || !listings.Any())
        //            return Ok(new List<SearchHomeListingViewModel>());

        //        listings = listings.Where(x => x.Address != null).ToList();
        //        if (!listings.Any())
        //            return Ok(new List<SearchHomeListingViewModel>());

        //        // Step 2: Prepare to match listings based on search text
        //        searchText = searchText.ToLower();
        //        var filteredListings = new List<ListingSearch>();

        //        foreach (var listing in listings)
        //        {
        //            bool isMatched = false;

        //            // Check if the CompanyName or Keyword contains the search text
        //            if (listing.CompanyName.ToLower().Contains(searchText) ||
        //                listing.Keyword.ToLower().Contains(searchText))
        //            {
        //                isMatched = true;
        //            }

        //            // Check if listing is in a category matching the search text
        //            if (!isMatched && await _searchRepository.IsListingInCategory(int.Parse(listing.ListingId), searchText))
        //            {
        //                isMatched = true;
        //            }

        //            // Check if any category matches the search text
        //            if (!isMatched)
        //            {
        //                var matchedCategory = await _searchRepository.GetCategoryByName(searchText);
        //                if (matchedCategory != null)
        //                {
        //                    matchingCategoryName = matchedCategory.Name;
        //                    dynamicCategoryId = matchedCategory.Id;
        //                    isMatched = true;
        //                }
        //            }

        //            // Add matched listing to filtered listings
        //            if (isMatched)
        //            {
        //                filteredListings.Add(listing);
        //            }
        //        }

        //        if (!filteredListings.Any())
        //            return Ok(new List<SearchHomeListingViewModel>());

        //        listings = filteredListings;
        //    }

        //    // Step 3: Retrieve locality details based on matched listings
        //    var locationIds = listings.Select(x => x.Address.AssemblyID).ToArray();
        //    var localities = await _searchRepository.GetLocalitiesByLocalityIds(locationIds);

        //    var result = new List<SearchHomeListingViewModel>();

        //    // Step 4: Create the result list with filtered listings
        //    foreach (var listing in listings)
        //    {
        //        // Get subcategories for each listing
        //        var subCategories = await _searchRepository.GetSubcategory(int.Parse(listing.ListingId));
        //        var subcategoryNames = subCategories.Select(sub => sub.Name).ToList();

        //        // Retrieve city and locality names
        //        var localityDetails = localities.FirstOrDefault(l => l.Id == listing.Address.AssemblyID);
        //        var city = localityDetails?.City.Name;
        //        var locality = localityDetails?.Name;

        //        result.Add(new SearchHomeListingViewModel
        //        {
        //            CompanyName = listing.CompanyName,
        //            listingId = listing.ListingId,
        //            ListingUrl = listing.ListingURL,
        //            CityName = city,
        //            LocalityName = locality,
        //            category = string.Join(", ", subcategoryNames),
        //            CategoryId = subCategories.FirstOrDefault()?.Id ?? 0,
        //            keyword = listing.Keyword,
        //            keywordId = listing.ListingId
        //        });
        //    }

        //    // Step 5: Add the dynamic category result if a category name matched
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
        //            CategoryId = dynamicCategoryId
        //        });
        //    }

        //    return Ok(result);
        //}

        private async Task<List<ListingSearch>> GetListingsByMobileNumber(string partialMobileNumber)
        {
            try
            {
                var listings = new List<ListingSearch>();
                string connectionString = _configuration.GetConnectionString("MimListing");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        SELECT L.ListingID, L.CompanyName, L.ListingURL, A.AssemblyID
                        FROM [listing].[Listing] L
                        INNER JOIN [listing].[Communication] C ON L.ListingID = C.ListingID
                        INNER JOIN [listing].[Address] A ON L.ListingID = A.ListingID
                        WHERE C.Mobile LIKE @PartialMobileNumber";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PartialMobileNumber", "%" + partialMobileNumber + "%");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                listings.Add(new ListingSearch
                                {
                                    ListingId = reader["ListingID"].ToString(),
                                    CompanyName = reader["CompanyName"].ToString(),
                                    ListingURL = reader["ListingURL"].ToString(),
                                    Address = new AddressSearch
                                    {
                                        AssemblyID = reader["AssemblyID"].ToString()
                                    }
                                });
                            }
                        }
                    }
                }
                return listings;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private async Task<List<ListingSearch>> GetListingsByGstNumber(string gstNumber)
        {
            try
            {
                var listings = new List<ListingSearch>();
                string connectionString = _configuration.GetConnectionString("MimListing");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        SELECT L.ListingID, L.CompanyName, L.ListingURL, A.AssemblyID
                        FROM [listing].[Listing] L
                        INNER JOIN [listing].[Address] A ON L.ListingID = A.ListingID
                        WHERE L.GSTNumber LIKE @GstNumber";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GstNumber", "%" + gstNumber + "%");
                       
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                listings.Add(new ListingSearch
                                {
                                    ListingId = reader["ListingID"].ToString(),
                                    CompanyName = reader["CompanyName"].ToString(),
                                    ListingURL = reader["ListingURL"].ToString(),
                                    Address = new AddressSearch
                                    {
                                        AssemblyID = reader["AssemblyID"].ToString()
                                    }
                                });
                            }
                        }
                    }
                }
                return listings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SearchHomeListingViewModel>>> SearchListings(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return Ok(new List<SearchHomeListingViewModel>());

            string matchingCategoryName = string.Empty;
            int dynamicCategoryId = 0;
            bool isMobileNumber = long.TryParse(searchText, out _) && searchText.Length >= 1;
            bool isGstNumber = Regex.IsMatch(searchText, @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$");
            List<ListingSearch> listings;

            if (isMobileNumber)
            {
                listings = await GetListingsByMobileNumber(searchText);
            }
            else if (isGstNumber)
            {
                listings = await GetListingsByGstNumber(searchText);
            }
            else
            {
                // Extract location and keyword from search text, if applicable
                string specialization = null;
                string keyword = null;
                string location = null;

                // Check for known location keywords
                if (Regex.IsMatch(searchText, @"\b(in|at|near|near me|near by)\b", RegexOptions.IgnoreCase))
                {
                    // Match everything before the keyword and everything after it as the location
                    var match = Regex.Match(searchText, @"^(.*)\b(in|at|near|near me|near by)\b\s*(.*)$", RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string keywordPart = match.Groups[1].Value.Trim(); // Keyword part
                        location = match.Groups[3].Value.Trim(); // Location part (after the keyword)

                        // Remove "me", "by", or any other extra words if they are part of the location
                        // Handle cases where location contains "near me", "near by", etc.
                        var locationKeywords = new List<string> { "me", "by" };
                        foreach (var keywords in locationKeywords)
                        {
                            if (location.StartsWith(keywords, StringComparison.OrdinalIgnoreCase))
                            {
                                location = location.Substring(keywords.Length).Trim(); // Remove keyword like "me", "by"
                            }
                        }

                        // List of known specializations
                        var specializations = new List<string> { "Banks", "BeautyParlors", "Bungalow", "CallCenter", "Church", "Company", "ComputerInstitute", "Dispensary", "ExhibitionStall", "Factory", "Farmhouse", "Gurudwara", "Gym", "HealthClub", "Home", "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "ResidentialSociety", "Resorts", "Restaurants", "Salons", "Shop", "ShoppingMall", "Showroom", "Warehouse", "AcceptTenderWork" };

                        // Check for specialization at the start of the keyword part
                        var specializationMatch = specializations.FirstOrDefault(s => keywordPart.StartsWith(s, StringComparison.OrdinalIgnoreCase));

                        if (specializationMatch != null)
                        {
                            specialization = specializationMatch;
                            keyword = keywordPart.Substring(specialization.Length).Trim(); // Remove specialization from keyword part
                        }
                        else
                        {
                            keyword = keywordPart; // Treat everything as keyword if no specialization is found
                        }
                    }
                }

                //if (searchText.Contains("near by"))
                //{
                //    var parts = searchText.Split(new[] { "near by" }, StringSplitOptions.RemoveEmptyEntries);
                //    if (parts.Length == 2)
                //    {
                //        string keywordPart = parts[0].Trim();
                //        location = parts[1].Trim();

                //        // List of known specializations
                //        var specializations = new List<string> { "Banks", "BeautyParlors", "Bungalow", "CallCenter", "Church", "Company", "ComputerInstitute", "Dispensary", "ExhibitionStall", "Factory", "Farmhouse", "Gurudwara", "Gym", "HealthClub", "Home", "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "ResidentialSociety", "Resorts", "Restaurants", "Salons", "Shop", "ShoppingMall", "Showroom", "Warehouse", "AcceptTenderWork" };

                //        // Check if any known specialization is at the start of the keyword part
                //        var specializationMatch = specializations.FirstOrDefault(s => keywordPart.StartsWith(s, StringComparison.OrdinalIgnoreCase));

                //        if (specializationMatch != null)
                //        {
                //            specialization = specializationMatch;
                //            // Remove the specialization from the keyword part to get the actual keyword
                //            keyword = keywordPart.Substring(specialization.Length).Trim();
                //        }
                //        else
                //        {
                //            // If no specialization is found, treat the entire keyword part as the keyword
                //            keyword = keywordPart;
                //        }
                //    }
                //}
                //else if (searchText.Contains("near me"))
                //{
                //    var parts = searchText.Split(new[] { "near me" }, StringSplitOptions.RemoveEmptyEntries);
                //    if (parts.Length == 2)
                //    {
                //        string keywordPart = parts[0].Trim();
                //        location = parts[1].Trim();

                //        // List of known specializations
                //        var specializations = new List<string> { "Banks", "BeautyParlors", "Bungalow", "CallCenter", "Church", "Company", "ComputerInstitute", "Dispensary", "ExhibitionStall", "Factory", "Farmhouse", "Gurudwara", "Gym", "HealthClub", "Home", "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "ResidentialSociety", "Resorts", "Restaurants", "Salons", "Shop", "ShoppingMall", "Showroom", "Warehouse", "AcceptTenderWork" };

                //        // Check if any known specialization is at the start of the keyword part
                //        var specializationMatch = specializations.FirstOrDefault(s => keywordPart.StartsWith(s, StringComparison.OrdinalIgnoreCase));

                //        if (specializationMatch != null)
                //        {
                //            specialization = specializationMatch;
                //            // Remove the specialization from the keyword part to get the actual keyword
                //            keyword = keywordPart.Substring(specialization.Length).Trim();
                //        }
                //        else
                //        {
                //            // If no specialization is found, treat the entire keyword part as the keyword
                //            keyword = keywordPart;
                //        }
                //    }
                //}
                //else if (searchText.Contains("near"))
                //{
                //    var parts = searchText.Split(new[] { "near" }, StringSplitOptions.RemoveEmptyEntries);
                //    if (parts.Length == 2)
                //    {
                //        string keywordPart = parts[0].Trim();
                //        location = parts[1].Trim();

                //        // List of known specializations
                //        var specializations = new List<string> { "Banks", "BeautyParlors", "Bungalow", "CallCenter", "Church", "Company", "ComputerInstitute", "Dispensary", "ExhibitionStall", "Factory", "Farmhouse", "Gurudwara", "Gym", "HealthClub", "Home", "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "ResidentialSociety", "Resorts", "Restaurants", "Salons", "Shop", "ShoppingMall", "Showroom", "Warehouse", "AcceptTenderWork" };

                //        // Check if any known specialization is at the start of the keyword part
                //        var specializationMatch = specializations.FirstOrDefault(s => keywordPart.StartsWith(s, StringComparison.OrdinalIgnoreCase));

                //        if (specializationMatch != null)
                //        {
                //            specialization = specializationMatch;
                //            // Remove the specialization from the keyword part to get the actual keyword
                //            keyword = keywordPart.Substring(specialization.Length).Trim();
                //        }
                //        else
                //        {
                //            // If no specialization is found, treat the entire keyword part as the keyword
                //            keyword = keywordPart;
                //        }
                //    }
                //}
                //else if (searchText.Contains("at"))
                //{
                //    var parts = searchText.Split(new[] { "at" }, StringSplitOptions.RemoveEmptyEntries);
                //    if (parts.Length == 2)
                //    {
                //        string keywordPart = parts[0].Trim();
                //        location = parts[1].Trim();

                //        // List of known specializations
                //        var specializations = new List<string> { "Banks", "BeautyParlors", "Bungalow", "CallCenter", "Church", "Company", "ComputerInstitute", "Dispensary", "ExhibitionStall", "Factory", "Farmhouse", "Gurudwara", "Gym", "HealthClub", "Home", "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "ResidentialSociety", "Resorts", "Restaurants", "Salons", "Shop", "ShoppingMall", "Showroom", "Warehouse", "AcceptTenderWork" };

                //        // Check if any known specialization is at the start of the keyword part
                //        var specializationMatch = specializations.FirstOrDefault(s => keywordPart.StartsWith(s, StringComparison.OrdinalIgnoreCase));

                //        if (specializationMatch != null)
                //        {
                //            specialization = specializationMatch;
                //            // Remove the specialization from the keyword part to get the actual keyword
                //            keyword = keywordPart.Substring(specialization.Length).Trim();
                //        }
                //        else
                //        {
                //            // If no specialization is found, treat the entire keyword part as the keyword
                //            keyword = keywordPart;
                //        }
                //    }
                //}
                //else if (searchText.Contains("in"))
                //{
                //    var parts = searchText.Split(new[] { "in" }, StringSplitOptions.RemoveEmptyEntries);
                //    if (parts.Length == 2)
                //    {
                //        string keywordPart = parts[0].Trim();
                //        location = parts[1].Trim();

                //        // List of known specializations
                //        var specializations = new List<string> { "Banks", "BeautyParlors", "Bungalow", "CallCenter", "Church", "Company", "ComputerInstitute", "Dispensary", "ExhibitionStall", "Factory", "Farmhouse", "Gurudwara", "Gym", "HealthClub", "Home", "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "ResidentialSociety", "Resorts", "Restaurants", "Salons", "Shop", "ShoppingMall", "Showroom", "Warehouse", "AcceptTenderWork" };

                //        // Check if any known specialization is at the start of the keyword part
                //        var specializationMatch = specializations.FirstOrDefault(s => keywordPart.StartsWith(s, StringComparison.OrdinalIgnoreCase));

                //        if (specializationMatch != null)
                //        {
                //            specialization = specializationMatch;
                //            // Remove the specialization from the keyword part to get the actual keyword
                //            keyword = keywordPart.Substring(specialization.Length).Trim();
                //        }
                //        else
                //        {
                //            // If no specialization is found, treat the entire keyword part as the keyword
                //            keyword = keywordPart;
                //        }
                //    }
                //}


                if (!string.IsNullOrEmpty(location))
                {
                    // Use the SQL join query to search for listings based on keyword and location
                    listings = await SearchByKeywordAndLocation(keyword, location);
                }
                else
                {
                    listings = (await _searchRepository.GetApprovedListings()).ToList();
                    if (listings == null || !listings.Any())
                        return Ok(new List<SearchHomeListingViewModel>());

                    listings = listings.Where(x => x.Address != null).ToList();
                    if (!listings.Any())
                        return Ok(new List<SearchHomeListingViewModel>());

                    searchText = searchText.ToLower();
                    var filteredListings = new List<ListingSearch>();

                    foreach (var listing in listings)
                    {
                        bool isMatched = listing.CompanyName.ToLower().Contains(searchText) ||
                                         listing.Keyword.ToLower().Contains(searchText) ||
                                         (await _searchRepository.IsListingInCategory(int.Parse(listing.ListingId), searchText));

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

                        if (isMatched)
                        {
                            filteredListings.Add(listing);
                        }
                    }

                    // New logic to check for keywords in the Keyword table
                    var keywordResults = await GetListingsByKeyword(searchText);
                    foreach (var keywordListing in keywordResults)
                    {
                        if (!filteredListings.Any(fl => fl.ListingId == keywordListing.ListingId))
                        {
                            filteredListings.Add(keywordListing);
                        }
                    }

                    if (!filteredListings.Any())
                        return Ok(new List<SearchHomeListingViewModel>());

                    listings = filteredListings;
                }
            }

            var locationIds = listings.Select(x => x.Address.AssemblyID).ToArray();
            var localities = await _searchRepository.GetLocalitiesByLocalityIds(locationIds);
            var result = new List<SearchHomeListingViewModel>();

            foreach (var listing in listings)
            {
                var subCategories = await _searchRepository.GetSubcategory(int.Parse(listing.ListingId));
                var subcategoryNames = subCategories.Select(sub => sub.Name).ToList();
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

        private async Task<List<ListingSearch>> SearchByKeywordAndLocation(string keyword, string location, string specialization = null)
        {
            var results = new List<ListingSearch>();
            //string query = @"
            //    SELECT l.ListingID, l.CompanyName, l.ListingURL, c.Name as City, loc.Name as Locality, 
            //    k.KeywordID, k.SeoKeyword, a.AssemblyID
            //    FROM [dbo].[Keyword] k
            //    INNER JOIN [listing].[Address] a ON k.ListingID = a.ListingID
            //    INNER JOIN [listing].[Listing] l ON a.ListingID = l.ListingID
            //    INNER JOIN [MimShared].[shared].[City] c ON a.City = c.CityID
            //    INNER JOIN [MimShared].[dbo].[Location] loc ON a.AssemblyID = loc.Id
            //    INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
            //    WHERE k.SeoKeyword LIKE '%' + @Keyword + '%'
            //    AND (c.Name LIKE '%' + @Location + '%' OR loc.Name LIKE '%' + @Location + '%')
            //    AND (@Specialization IS NULL OR (@Specialization = 'Banks' AND s.Banks = 1) OR (@Specialization = 'BeautyParlors' AND s.BeautyParlors = 1) OR (@Specialization = 'Bungalow' AND s.Bungalow = 1) OR (@Specialization = 'CallCenter' AND s.CallCenter = 1) OR (@Specialization = 'Church' AND s.Church = 1) OR (@Specialization = 'Company' AND s.Company = 1) OR (@Specialization = 'ComputerInstitute' AND s.ComputerInstitute = 1) OR (@Specialization = 'Dispensary' AND s.Dispensary = 1) OR (@Specialization = 'ExhibitionStall' AND s.ExhibitionStall = 1) OR (@Specialization = 'Factory' AND s.Factory = 1) OR (@Specialization = 'Farmhouse' AND s.Farmhouse = 1) OR (@Specialization = 'Gurudwara' AND s.Gurudwara = 1) OR (@Specialization = 'Gym' AND s.Gym = 1) OR (@Specialization = 'HealthClub' AND s.HealthClub = 1) OR (@Specialization = 'Home' AND s.Home = 1) OR (@Specialization = 'Hospital' AND s.Hospital = 1) OR (@Specialization = 'Hotel' AND s.Hotel = 1) OR (@Specialization = 'Laboratory' AND s.Laboratory = 1) OR (@Specialization = 'Mandir' AND s.Mandir = 1) OR (@Specialization = 'Mosque' AND s.Mosque = 1) OR (@Specialization = 'Office' AND s.Office = 1) OR (@Specialization = 'Plazas' AND s.Plazas = 1) OR (@Specialization = 'ResidentialSociety' AND s.ResidentialSociety = 1) OR (@Specialization = 'Resorts' AND s.Resorts = 1) OR (@Specialization = 'Restaurants' AND s.Restaurants = 1) OR (@Specialization = 'Salons' AND s.Salons = 1) OR (@Specialization = 'Shop' AND s.Shop = 1) OR (@Specialization = 'ShoppingMall' AND s.ShoppingMall = 1) OR (@Specialization = 'Showroom' AND s.Showroom = 1) OR (@Specialization = 'Warehouse' AND s.Warehouse = 1) OR (@Specialization = 'AcceptTenderWork' AND s.AcceptTenderWork = 1));";


            string query = @"
                SELECT l.ListingID, l.CompanyName, l.ListingURL, c.Name as City, loc.Name as Locality, 
                k.KeywordID, k.SeoKeyword, a.AssemblyID
                FROM [dbo].[Keyword] k
                INNER JOIN [listing].[Address] a ON k.ListingID = a.ListingID
                INNER JOIN [listing].[Listing] l ON a.ListingID = l.ListingID
                INNER JOIN [MimShared_Api].[shared].[City] c ON a.City = c.CityID
                INNER JOIN [MimShared_Api].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                WHERE k.SeoKeyword LIKE '%' + @Keyword + '%'
                AND (c.Name LIKE '%' + @Location + '%' OR loc.Name LIKE '%' + @Location + '%')
                AND (@Specialization IS NULL OR (@Specialization = 'Banks' AND s.Banks = 1) OR (@Specialization = 'BeautyParlors' AND s.BeautyParlors = 1) OR (@Specialization = 'Bungalow' AND s.Bungalow = 1) OR (@Specialization = 'CallCenter' AND s.CallCenter = 1) OR (@Specialization = 'Church' AND s.Church = 1) OR (@Specialization = 'Company' AND s.Company = 1) OR (@Specialization = 'ComputerInstitute' AND s.ComputerInstitute = 1) OR (@Specialization = 'Dispensary' AND s.Dispensary = 1) OR (@Specialization = 'ExhibitionStall' AND s.ExhibitionStall = 1) OR (@Specialization = 'Factory' AND s.Factory = 1) OR (@Specialization = 'Farmhouse' AND s.Farmhouse = 1) OR (@Specialization = 'Gurudwara' AND s.Gurudwara = 1) OR (@Specialization = 'Gym' AND s.Gym = 1) OR (@Specialization = 'HealthClub' AND s.HealthClub = 1) OR (@Specialization = 'Home' AND s.Home = 1) OR (@Specialization = 'Hospital' AND s.Hospital = 1) OR (@Specialization = 'Hotel' AND s.Hotel = 1) OR (@Specialization = 'Laboratory' AND s.Laboratory = 1) OR (@Specialization = 'Mandir' AND s.Mandir = 1) OR (@Specialization = 'Mosque' AND s.Mosque = 1) OR (@Specialization = 'Office' AND s.Office = 1) OR (@Specialization = 'Plazas' AND s.Plazas = 1) OR (@Specialization = 'ResidentialSociety' AND s.ResidentialSociety = 1) OR (@Specialization = 'Resorts' AND s.Resorts = 1) OR (@Specialization = 'Restaurants' AND s.Restaurants = 1) OR (@Specialization = 'Salons' AND s.Salons = 1) OR (@Specialization = 'Shop' AND s.Shop = 1) OR (@Specialization = 'ShoppingMall' AND s.ShoppingMall = 1) OR (@Specialization = 'Showroom' AND s.Showroom = 1) OR (@Specialization = 'Warehouse' AND s.Warehouse = 1) OR (@Specialization = 'AcceptTenderWork' AND s.AcceptTenderWork = 1));";


            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Keyword", keyword);
                    command.Parameters.AddWithValue("@Location", location);
                    command.Parameters.AddWithValue("@Specialization", (object)specialization ?? DBNull.Value);

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        await connection.OpenAsync();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            results.Add(new ListingSearch
                            {
                                ListingId = row["ListingID"].ToString(),
                                CompanyName = row["CompanyName"].ToString(),
                                ListingURL = row["ListingURL"].ToString(),
                                City = row["City"].ToString(),
                                Locality = row["Locality"].ToString(),
                                KeywordID = Convert.ToInt32(row["KeywordID"]),
                                Keyword = row["SeoKeyword"].ToString(),
                                Address = new AddressSearch
                                {
                                    AssemblyID = row["AssemblyID"].ToString()
                                }
                            });
                        }
                    }
                }
            }

            return results;
        }

        private async Task<List<ListingSearch>> GetListingsByKeyword(string keyword)
        {
            var listings = new List<ListingSearch>();
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT k.ListingID, l.CompanyName, l.ListingURL, k.SeoKeyword, a.AssemblyID
                FROM [dbo].[Keyword] k
                LEFT JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                LEFT JOIN [listing].[Address] a ON k.ListingID = a.ListingID
                WHERE k.SeoKeyword LIKE '%' + @Keyword + '%'";


                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Keyword", SqlDbType.NVarChar) { Value = keyword });

                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var listing = new ListingSearch
                            {
                                ListingId = reader["ListingID"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                ListingURL = reader["ListingURL"].ToString(),
                                Keyword = reader["SeoKeyword"].ToString(),
                                Address = new AddressSearch
                                {
                                    AssemblyID = reader["AssemblyID"].ToString()
                                }
                            };

                            listings.Add(listing);
                        }
                    }
                }
            }

            return listings;
        }
    }
}
