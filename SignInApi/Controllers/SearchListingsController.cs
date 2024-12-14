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
                        var specializations = new List<string> { "Banks", "Beauty Parlors", "Bungalow", "Call Center", "Church", "Company", "Computer Institute", "Dispensary", "Exhibition Stall", "Factory", "Farmhouse", "Gurudwara", "Gym", "Health Club", "Home", "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "Residential Society", "Resorts", "Restaurants", "Salons", "Shop", "Shopping Mall", "Showroom", "Warehouse", "Accept Tender Work" };

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

                if (!string.IsNullOrEmpty(location))
                {
                    // Use the SQL join query to search for listings based on keyword and location
                    listings = await SearchByKeywordAndLocation(keyword, location,specialization);
                }
                else
                {
                    listings = (await _searchRepository.GetApprovedListings(searchText)).ToList();
                    //if (listings == null || !listings.Any())
                    //    return Ok(new List<SearchHomeListingViewModel>());

                    listings = listings.Where(x => x.Address != null).ToList();
                    //if (!listings.Any())
                    //    return Ok(new List<SearchHomeListingViewModel>());
 
                    searchText = searchText.ToLower();
                    var filteredListings = new List<ListingSearch>();

                    foreach (var listing in listings)
                    {
                        bool isMatched = listing.CompanyName?.ToLower().Contains(searchText) == true || // Partial match in company name
                             listing.Keyword?.ToLower().Contains(searchText) == true ||     // Partial match in keyword
                             (listing.Address?.AssemblyID?.ToLower().Contains(searchText) == true) || // Match in address
                             (await _searchRepository.IsListingInCategory(int.Parse(listing.ListingId), searchText)); // Match in category

                        if (isMatched)
                        {
                            // Agar category name match kare to usse add karein
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

                    // Specialization and Keyword Logic
                    var specializationListings = await GetKeywordSuggestionsBySpecialization(searchText);
                    foreach (var specListing in specializationListings)
                    {
                        if (int.TryParse(specListing.ListingId, out int listingId))
                        {
                            filteredListings.Add(specListing);                            
                        }
                    }

                    var specializationKeywordListings = await GetLocationSuggestionsBySpecializationAndKeyword(searchText);
                    foreach (var speckeywordListing in specializationKeywordListings)
                    {
                        if (int.TryParse(speckeywordListing.ListingId, out int listingId))
                        { 
                            filteredListings.Add(speckeywordListing);
                        }
                    }

                    // New logic to check for keywords in the Keyword table
                    var keywordResults = await GetListingsByKeyword(searchText);
                    foreach (var keywordListing in keywordResults)
                    {
                        if (string.IsNullOrEmpty(keywordListing.ListingId) || filteredListings.Any(fl => fl.ListingId == keywordListing.ListingId))
                        {
                            filteredListings.Add(keywordListing);
                        }
                    }

                    var ownerResults = await GetListingsByOwnername(searchText);
                    foreach (var ownerListing in ownerResults)
                    {
                        if (!string.IsNullOrWhiteSpace(ownerListing.ListingId) || filteredListings.Any(fl => fl.ListingId == ownerListing.ListingId))
                        {
                            filteredListings.Add(ownerListing);
                        }
                    }

                    // Add this line here to remove duplicates
                    //filteredListings = filteredListings.GroupBy(x => x.ListingId).Select(g => g.First()).ToList();

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
                var subCategories = await _searchRepository.GetSubcategory(int.TryParse(listing.ListingId, out var listingId) ? listingId : 0);
                var subcategoryNames = subCategories.Select(sub => sub.Name).ToList();
                var localityDetails = localities.FirstOrDefault(l => l.Id == listing.Address.AssemblyID);
                var city = localityDetails?.City.Name;
                var locality = localityDetails?.Name;
                var area = localityDetails?.AreaName;

                result.Add(new SearchHomeListingViewModel
                {
                    CompanyName = listing.CompanyName,
                    listingId = listing.ListingId,
                    ListingUrl = listing.ListingURL,
                    CityName = city,
                    LocalityName = locality,
                    AreaName = area,
                    category = string.Join(", ", subcategoryNames),
                    CategoryId = subCategories.FirstOrDefault()?.Id ?? 0,
                    keyword = listing.Keyword,
                    keywordId = listing.ListingId,
                    Mobilenumber = listing.Mobilenumber,
                    Gstnumber = listing.Gstnumber,
                    Ownername = listing.Ownername,
                    Specialization = listing.Specialiazation,
                    OwnerPrefix = listing.OwnerPrefix,
                    OwnerLastname = listing.OwnerLastname,
                    allspecialiazationkeyword = listing.allspecialiazationkeyword
                });
            }

            // Sort results based on priority
            var firstObject = new List<SearchHomeListingViewModel>();
            if (!string.IsNullOrEmpty(matchingCategoryName))
            {
                firstObject.Add(new SearchHomeListingViewModel
                {
                    CompanyName = null,
                    listingId = null,
                    ListingUrl = null,
                    CityName = null,
                    LocalityName = null,
                    AreaName = null,
                    category = matchingCategoryName,
                    CategoryId = dynamicCategoryId,
                    keyword = null,
                    keywordId = null,
                    Mobilenumber = null,
                    Gstnumber = null,
                    Ownername = null,
                    Specialization = null,
                    allspecialiazationkeyword = null
                });
            }


            var ownerNameMatched = result
            .Where(x => x.Ownername != null && x.Ownername.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .GroupBy(x => x.Ownername)
            .Select(group => new
            {
                Ownername = group.Key,
                Listings = group.Select(item => new
                {
                    item.listingId,
                    item.CompanyName,
                    item.ListingUrl,
                    item.CityName,
                    item.LocalityName,
                    item.AreaName,
                    item.category,
                    item.CategoryId,
                    item.Mobilenumber,
                    item.Gstnumber,
                    item.Specialization,
                    item.OwnerPrefix,
                    item.OwnerLastname
                }).ToList(),
                ListingsCount = group.Count() // Total listings count for sorting
            })
            .OrderByDescending(x => x.ListingsCount) // Sort by number of listings in descending order
            .ThenBy(x => x.Ownername) // Secondary sort alphabetically by Ownername
            .ToList();

            var AllSearchkeywordMatched = result
            .Where(x => !string.IsNullOrEmpty(x.allspecialiazationkeyword)
                        && x.allspecialiazationkeyword.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .GroupBy(x => x.allspecialiazationkeyword.Trim()) // Trim to avoid grouping mismatches
            .Select(group => new
            {
                Allspecialiazationkeyword = group.Key,
                Listings = group.Select(item => new
                {
                    item.listingId,
                    item.CompanyName,
                    item.ListingUrl,
                    item.CityName,
                    item.LocalityName,
                    item.AreaName,
                    item.category,
                    item.CategoryId,
                    item.Mobilenumber,
                    item.Gstnumber,
                    item.Specialization,
                    item.OwnerPrefix
                }).ToList(),
                ListingsCount = group.Count()
            })
            .Where(x => !string.IsNullOrEmpty(x.Allspecialiazationkeyword)) // Ensure valid group keys
            .OrderByDescending(x => x.ListingsCount)
            .ThenBy(x => x.Allspecialiazationkeyword)
            .ToList();

            var keywordMatched = result
            .Where(x => x.keyword != null && x.keyword.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.keyword.Split('\n').Select(Keywords => new
            {
                Keyword = Keywords.Trim(),
                Listings =  new
                {
                    listingId = x.listingId,
                    companyName = x.CompanyName,
                    listingUrl = x.ListingUrl,
                    cityName = x.CityName,
                    localityName = x.LocalityName,
                    areaName = x.AreaName,
                    category = x.category,
                    categoryId = x.CategoryId,
                    mobilenumber = x.Mobilenumber,
                    gstnumber = x.Gstnumber,
                    ownername = x.Ownername
                }
            }))
           .GroupBy(x => x.Keyword)
           .Select(group => new
           {
               Keyword = group.Key,
               listings = group.Select(item => item.Listings).ToList(),
               listingsCount = group.Count()
           })
           .ToList();

            var specializationMatched = result
            .Where(x => x.Specialization != null && x.Specialization.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.Specialization.Split('\n').Select(specialization => new
            {
                Specialization = specialization.Trim(),
                Listing = new
                {
                    listingId = x.listingId,
                    companyName = x.CompanyName,
                    listingUrl = x.ListingUrl,
                    cityName = x.CityName,
                    localityName = x.LocalityName,
                    areaName = x.AreaName,
                    category = x.category,
                    categoryId = x.CategoryId,
                    mobilenumber = x.Mobilenumber,
                    gstnumber = x.Gstnumber,
                    ownername = x.Ownername
                }
            }))
            .GroupBy(x => x.Specialization)
            .Select(group => new
            {
                specialization = group.Key,
                listings = group.Select(item => item.Listing).ToList(),
                listingsCount = group.Count()
            })
            .ToList();


            var companyNameMatched = result
                .Where(x => x.CompanyName != null && x.CompanyName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.CompanyName)
                .DistinctBy(x => x.listingId) // Ensure unique entries by listingId
                .ToList();

            var sortedResponse = new
            {
                MatchedCategory = firstObject.Any() ? firstObject.First() : null,
                Keywords = keywordMatched, // Grouped by keyword
                SpecializationMatches = specializationMatched,
                AllspecializationandKeyword = AllSearchkeywordMatched,
                OwnernameMatches = ownerNameMatched,
                CompanyNameMatches = companyNameMatched.Select(item => new
                {
                    item.listingId,
                    item.CompanyName,
                    item.ListingUrl,
                    item.CityName,
                    item.LocalityName,
                    item.AreaName,
                    item.category,
                    item.CategoryId,
                    item.Mobilenumber,
                    item.Gstnumber,
                    item.Ownername,
                    item.Specialization,
                    item.allspecialiazationkeyword
                }).ToList()
            };


            // Check if the response contains any data
            bool hasData = sortedResponse.MatchedCategory != null
                || sortedResponse.Keywords.Any()
                || sortedResponse.CompanyNameMatches.Any()
                || sortedResponse.SpecializationMatches.Any()
                || sortedResponse.OwnernameMatches.Any()
                || sortedResponse.AllspecializationandKeyword.Any();

            if (!hasData)
                return Ok(result);

            return Ok(sortedResponse);
        }
         
        private async Task<List<ListingSearch>> SearchByKeywordAndLocation(string keyword, string location, string specialization = null)
        {
            var listing = new List<ListingSearch>();
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
                            listing.Add(new ListingSearch
                            {
                                ListingId = row["ListingID"].ToString(),
                                CompanyName = row["CompanyName"].ToString(),
                                ListingURL = row["ListingURL"].ToString(),
                                City = row["City"].ToString(),
                                Locality = row["Locality"].ToString(),
                                KeywordID = Convert.ToInt32(row["KeywordID"]),
                                allspecialiazationkeyword = $"{specialization} {keyword} in {location}",
                                Address = new AddressSearch
                                {
                                    AssemblyID = row["AssemblyID"].ToString()
                                }
                            });
                        }
                    }
                }
            }

            return listing;
        }


        private async Task<List<ListingSearch>> GetListingsByKeyword(string keyword)
        {
            try
            {
                var listings = new List<ListingSearch>();
                string connectionString = _configuration.GetConnectionString("MimListing");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    //string Cityquery = @"SELECT TOP 3 k.SeoKeyword, c.Name AS CityName, lst.ListingID, lst.CompanyName, lst.ListingURL, a.AssemblyID
                    //    FROM [dbo].[Keyword] k
                    //    LEFT JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    //    INNER JOIN [MimShared].[shared].[City] c ON a.City = c.CityID
                    //    LEFT JOIN [listing].[Listing] lst ON a.ListingId = lst.ListingId
                    //    WHERE k.SeoKeyword LIKE '%' + @Keyword + '%' ORDER BY k.SeoKeyword";

                    //string Localityquery = @"SELECT TOP 3 k.SeoKeyword, loc.Name AS LocationName, lst.ListingID, lst.CompanyName, lst.ListingURL, a.AssemblyID
                    //    FROM [dbo].[Keyword] k
                    //    LEFT JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    //    INNER JOIN [MimShared].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                    //    LEFT JOIN [listing].[Listing] lst ON a.ListingId = lst.ListingId
                    //    WHERE k.SeoKeyword LIKE '%' + @Keyword + '%' ORDER BY k.SeoKeyword";

                    //string Areaquery = @"SELECT TOP 3 k.SeoKeyword, ar.Name AS AreaName, lst.ListingID, lst.CompanyName, lst.ListingURL, a.AssemblyID
                    //    FROM [dbo].[Keyword] k
                    //    LEFT JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    //    INNER JOIN [MimShared].[dbo].[Area] ar ON a.LocalityID = ar.Id
                    //    LEFT JOIN [listing].[Listing] lst ON a.ListingId = lst.ListingId
                    //    WHERE k.SeoKeyword LIKE '%' + @Keyword + '%' ORDER BY k.SeoKeyword";


                    /// Server Query 
                    string Cityquery = @"SELECT TOP 3 k.SeoKeyword, c.Name AS CityName, lst.ListingID, lst.CompanyName, lst.ListingURL, a.AssemblyID
                        FROM [dbo].[Keyword] k
                        LEFT JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                        INNER JOIN [MimShared_Api].[shared].[City] c ON a.City = c.CityID
                        LEFT JOIN [listing].[Listing] lst ON a.ListingId = lst.ListingId
                        WHERE k.SeoKeyword LIKE '%' + @Keyword + '%' ORDER BY k.SeoKeyword";

                    string Localityquery = @"SELECT TOP 3 k.SeoKeyword, loc.Name AS LocationName, lst.ListingID, lst.CompanyName, lst.ListingURL, a.AssemblyID
                        FROM [dbo].[Keyword] k
                        LEFT JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                        INNER JOIN [MimShared_Api].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                        LEFT JOIN [listing].[Listing] lst ON a.ListingId = lst.ListingId
                        WHERE k.SeoKeyword LIKE '%' + @Keyword + '%' ORDER BY k.SeoKeyword";

                    string Areaquery = @"SELECT TOP 3 k.SeoKeyword, ar.Name AS AreaName, lst.ListingID, lst.CompanyName, lst.ListingURL, a.AssemblyID
                        FROM [dbo].[Keyword] k
                        LEFT JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                        INNER JOIN [MimShared_Api].[dbo].[Area] ar ON a.LocalityID = ar.Id
                        LEFT JOIN [listing].[Listing] lst ON a.ListingId = lst.ListingId
                        WHERE k.SeoKeyword LIKE '%' + @Keyword + '%' ORDER BY k.SeoKeyword";


                    async Task ExecuteQuery(string query, Action<SqlDataReader> processReader)
                    {
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    processReader(reader);
                                }
                            }
                        }
                    }

                    await ExecuteQuery(Cityquery, reader =>
                    {
                        listings.Add(new ListingSearch
                        {
                            City = reader["CityName"]?.ToString(),
                            Keyword = $"{reader["SeoKeyword"]} in {reader["CityName"]}",
                            CompanyName = reader["CompanyName"]?.ToString(),
                            ListingURL = reader["ListingURL"]?.ToString(),
                            ListingId = reader["ListingID"]?.ToString(),
                            Address = new AddressSearch
                            {
                                AssemblyID = reader["AssemblyID"]?.ToString()
                            }
                        }); ;
                    });

                    await ExecuteQuery(Localityquery, reader =>
                    {
                        listings.Add(new ListingSearch
                        {
                            Locality = reader["LocationName"]?.ToString(),
                            Keyword = $"{reader["SeoKeyword"]} in {reader["LocationName"]}",
                            CompanyName = reader["CompanyName"]?.ToString(),
                            ListingURL = reader["ListingURL"]?.ToString(),
                            ListingId = reader["ListingID"]?.ToString(),
                            Address = new AddressSearch
                            {
                                AssemblyID = reader["AssemblyID"]?.ToString()
                            }
                        }); ;
                    });

                    await ExecuteQuery(Areaquery, reader =>
                    {
                        listings.Add(new ListingSearch
                        {
                            Locality = reader["AreaName"]?.ToString(),
                            Keyword = $"{reader["SeoKeyword"]} in {reader["AreaName"]}",
                            CompanyName = reader["CompanyName"]?.ToString(),
                            ListingURL = reader["ListingURL"]?.ToString(),
                            ListingId = reader["ListingID"]?.ToString(),
                            Address = new AddressSearch
                            {
                                AssemblyID = reader["AssemblyID"]?.ToString()
                            }
                        }); ;
                    });
                }

                return listings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<List<ListingSearch>> GetKeywordSuggestionsBySpecialization(string searchText)
        {
            try
            {
                // Predefined list of specialization names
                var specializations = new List<string>
                {
                    "Banks", "Beauty Parlors", "Bungalow", "Call Center", "Church", "Company", "Computer Institute",
                    "Dispensary", "Exhibition Stall", "Factory", "Farmhouse", "Gurudwara", "Gym", "Health Club", "Home",
                    "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "Residential Society",
                    "Resorts", "Restaurants", "Salons", "Shop", "Shopping Mall", "Showroom", "Warehouse", "Accept Tender Work"
                };

                // Split specialization name and keyword from searchText
                string specializationName = string.Empty;
                string keyword = string.Empty;

                foreach (var specialization in specializations)
                {
                    if (searchText.StartsWith(specialization, StringComparison.OrdinalIgnoreCase))
                    {
                        specializationName = specialization;
                        keyword = searchText.Substring(specialization.Length).Trim();
                        break;
                    }
                }

                // If no specialization found, return an empty list
                if (!string.IsNullOrEmpty(keyword))
                {
                    return new List<ListingSearch>();
                }

                var listings = new List<ListingSearch>();
                string connectionString = _configuration.GetConnectionString("MimListing");

                // Sanitize specialization column name
                string specializationColumn = specializationName.Replace(" ", ""); // Remove spaces for column names

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the column exists in the Specialisation table
                    string checkColumnQuery = @"
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'listing' 
                    AND TABLE_NAME = 'Specialisation'
                    AND COLUMN_NAME = @ColumnName";

                    bool columnExists;
                    using (var checkCommand = new SqlCommand(checkColumnQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ColumnName", specializationColumn);
                        columnExists = (await checkCommand.ExecuteScalarAsync()) != null;
                    }

                    // Construct the query dynamically
                    string query;
                    if (columnExists)
                    {
                        // Column exists, include it in the query
                        query = $@"
                        SELECT DISTINCT 
                        k.SeoKeyword,
                        l.CompanyName,
                        l.ListingURL,
                        l.ListingID,
                        a.AssemblyID,
                        s.*
                        FROM [dbo].[Keyword] k
                        INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                        INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                        INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                        WHERE s.{specializationColumn} = 1;";
                    }
                    else
                    {
                        // Column does not exist, return NULL as SpecializationValue
                        query = @"
                        SELECT DISTINCT 
                        k.SeoKeyword,
                        l.CompanyName,
                        l.ListingURL,
                        l.ListingID,
                        a.AssemblyID,
                        NULL AS SpecializationValue
                        FROM [dbo].[Keyword] k
                        INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                        INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                        INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID;";
                    }

                    // Execute the query and process the results
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var listing = new ListingSearch
                            {
                                ListingId = reader["ListingID"]?.ToString(),
                                CompanyName = reader["CompanyName"]?.ToString(),
                                ListingURL = reader["ListingURL"]?.ToString(),
                                Keyword = reader["SeoKeyword"]?.ToString(),
                                Address = new AddressSearch
                                {
                                    AssemblyID = reader["AssemblyID"]?.ToString()
                                },
                                Specialiazation = columnExists
                                    ? $"{specializationName} {reader["SeoKeyword"]}"
                                    : null, // Null if the column doesn't exist
                            };

                            listings.Add(listing);
                        }
                    }
                }

                return listings;
            }
            catch (Exception ex)
            {
                // Log the exception (optional: use a logger)
                Console.WriteLine($"Error in GetKeywordSuggestionsBySpecialization: {ex.Message}");
                throw; // Preserve stack trace for further analysis
            }
        }
        

        private async Task<List<ListingSearch>> GetLocationSuggestionsBySpecializationAndKeyword(string searchText)
        {
            try
            {
                // Predefined list of specialization names
                var specializations = new List<string>
                {
                    "Banks", "Beauty Parlors", "Bungalow", "Call Center", "Church", "Company", "Computer Institute",
                    "Dispensary", "Exhibition Stall", "Factory", "Farmhouse", "Gurudwara", "Gym", "Health Club", "Home",
                    "Hospital", "Hotel", "Laboratory", "Mandir", "Mosque", "Office", "Plazas", "Residential Society",
                    "Resorts", "Restaurants", "Salons", "Shop", "Shopping Mall", "Showroom", "Warehouse", "Accept Tender Work"
                };

                // Extract specialization and keyword from searchText
                string specializationName = string.Empty;
                string keyword = string.Empty;

                foreach (var specialization in specializations)
                {
                    if (searchText.StartsWith(specialization, StringComparison.OrdinalIgnoreCase))
                    {
                        specializationName = specialization;
                        keyword = searchText.Substring(specialization.Length).Trim();
                        break;
                    }
                }

                // Return an empty list if no keyword is found
                if (string.IsNullOrEmpty(keyword))
                {
                    return new List<ListingSearch>();
                }

                var allListings = new List<ListingSearch>();
                string connectionString = _configuration.GetConnectionString("MimListing");

                // Sanitize the specialization column name
                string specializationColumn = specializationName.Replace(" ", "");

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the column exists in the Specialisation table
                    string checkColumnQuery = @"
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'listing' 
                    AND TABLE_NAME = 'Specialisation'
                    AND COLUMN_NAME = @ColumnName";

                    bool columnExists;
                    using (var checkCommand = new SqlCommand(checkColumnQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ColumnName", specializationColumn);
                        columnExists = (await checkCommand.ExecuteScalarAsync()) != null;
                    }

                    if (!columnExists)
                    {
                        return new List<ListingSearch>();
                    }

                    // Queries for City, Location, and Area data
                    //string cityQuery = $@"
                    //SELECT DISTINCT TOP 3 c.Name AS CityName, k.SeoKeyword, l.CompanyName, l.ListingURL, l.ListingID, a.City, a.AssemblyID, s.*
                    //FROM [dbo].[Keyword] k
                    //INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                    //INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    //INNER JOIN [MimShared].[shared].[City] c ON a.City = c.CityID
                    //INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                    //WHERE s.{specializationColumn} = 1 AND k.SeoKeyword LIKE @Keyword ORDER BY k.SeoKeyword";

                    //string locationQuery = $@"
                    //SELECT DISTINCT TOP 3 loc.Name AS LocationName, k.SeoKeyword, l.CompanyName, l.ListingURL, l.ListingID, a.City, a.AssemblyID, s.*
                    //FROM [dbo].[Keyword] k
                    //INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                    //INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    //INNER JOIN [MimShared].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                    //INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                    //WHERE s.{specializationColumn} = 1 AND k.SeoKeyword LIKE @Keyword ORDER BY k.SeoKeyword";

                    //string areaQuery = $@"
                    //SELECT DISTINCT TOP 3 ar.Name AS AreaName, k.SeoKeyword, l.CompanyName, l.ListingURL, l.ListingID, a.City, a.AssemblyID, s.*
                    //FROM [dbo].[Keyword] k
                    //INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                    //INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    //INNER JOIN [MimShared].[dbo].[Area] ar ON a.LocalityID = ar.Id
                    //INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                    //WHERE s.{specializationColumn} = 1 AND k.SeoKeyword LIKE @Keyword ORDER BY k.SeoKeyword";



                    //// Server Query
                    string cityQuery = $@"
                    SELECT DISTINCT TOP 3 c.Name AS CityName, k.SeoKeyword, l.CompanyName, l.ListingURL, l.ListingID, a.City, a.AssemblyID, s.*
                    FROM [dbo].[Keyword] k
                    INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                    INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    INNER JOIN [MimShared_Api].[shared].[City] c ON a.City = c.CityID
                    INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                    WHERE s.{specializationColumn} = 1 AND k.SeoKeyword LIKE @Keyword ORDER BY k.SeoKeyword";

                    string locationQuery = $@"
                    SELECT DISTINCT TOP 3 loc.Name AS LocationName, k.SeoKeyword, l.CompanyName, l.ListingURL, l.ListingID, a.City, a.AssemblyID, s.*
                    FROM [dbo].[Keyword] k
                    INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                    INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    INNER JOIN [MimShared_Api].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                    INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                    WHERE s.{specializationColumn} = 1 AND k.SeoKeyword LIKE @Keyword ORDER BY k.SeoKeyword";

                    string areaQuery = $@"
                    SELECT DISTINCT TOP 3 ar.Name AS AreaName, k.SeoKeyword, l.CompanyName, l.ListingURL, l.ListingID, a.City, a.AssemblyID, s.*
                    FROM [dbo].[Keyword] k
                    INNER JOIN [listing].[Listing] l ON k.ListingID = l.ListingID
                    INNER JOIN [listing].[Address] a ON k.ListingId = a.ListingId
                    INNER JOIN [MimShared_Api].[dbo].[Area] ar ON a.LocalityID = ar.Id
                    INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                    WHERE s.{specializationColumn} = 1 AND k.SeoKeyword LIKE @Keyword ORDER BY k.SeoKeyword";

                    // Execute the queries and process results
                    async Task ExecuteQuery(string query, Action<SqlDataReader> processReader)
                    {
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    processReader(reader);
                                }
                            }
                        }
                    }

                    await ExecuteQuery(cityQuery, reader =>
                    {
                        allListings.Add(new ListingSearch
                        {
                            City = reader["CityName"]?.ToString(),
                            Keyword = reader["SeoKeyword"]?.ToString(),
                            CompanyName = reader["CompanyName"]?.ToString(),
                            ListingURL = reader["ListingURL"]?.ToString(),
                            ListingId = reader["ListingID"]?.ToString(),
                            Address = new AddressSearch
                            {
                                AssemblyID = reader["AssemblyID"]?.ToString()
                            },
                            Specialiazation = $"{specializationName} {reader["SeoKeyword"]} in {reader["CityName"]}"
                        }); ;
                    });

                    await ExecuteQuery(locationQuery, reader =>
                    {
                        allListings.Add(new ListingSearch
                        {
                            Locality = reader["LocationName"]?.ToString(),
                            Keyword = reader["SeoKeyword"]?.ToString(),
                            CompanyName = reader["CompanyName"]?.ToString(),
                            ListingURL = reader["ListingURL"]?.ToString(),
                            ListingId = reader["ListingID"]?.ToString(),
                            Address = new AddressSearch
                            {
                                AssemblyID = reader["AssemblyID"]?.ToString()
                            },
                            Specialiazation = $"{specializationName} {reader["SeoKeyword"]} in {reader["LocationName"]}"
                        });
                    });

                    await ExecuteQuery(areaQuery, reader =>
                    {
                        allListings.Add(new ListingSearch
                        {
                            Area = reader["AreaName"]?.ToString(),
                            Keyword = reader["SeoKeyword"]?.ToString(),
                            CompanyName = reader["CompanyName"]?.ToString(),
                            ListingURL = reader["ListingURL"]?.ToString(),
                            ListingId = reader["ListingID"]?.ToString(),
                            Address = new AddressSearch
                            {
                                AssemblyID = reader["AssemblyID"]?.ToString()
                            },
                            Specialiazation = $"{specializationName} {reader["SeoKeyword"]} in {reader["AreaName"]}"
                        });
                    });
                }

                return allListings;
            }
            catch (Exception ex)
            {
                // Log the exception (replace with a logger if available)
                Console.WriteLine($"Error in GetLocationSuggestionsBySpecializationAndKeyword: {ex.Message}");
                throw; // Preserve the stack trace
            }
        }


        private async Task<List<ListingSearch>> GetListingsByOwnername(string ownername)
        {
            var listings = new List<ListingSearch>();
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT o.ListingID, l.CompanyName, l.ListingURL, o.MrndMs, o.OwnerName, o.LastName, a.AssemblyID
                FROM [dbo].[OwnerImage] o
                LEFT JOIN [listing].[Listing] l ON o.ListingID = l.ListingID
                LEFT JOIN [listing].[Address] a ON o.ListingID = a.ListingID
                WHERE o.OwnerName LIKE '%' + @ownername + '%'";


                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@ownername", SqlDbType.NVarChar) { Value = ownername });

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
                                Ownername = reader["OwnerName"].ToString(),
                                OwnerLastname = reader["LastName"].ToString(),
                                OwnerPrefix = reader["MrndMs"].ToString(),
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
                    SELECT L.ListingID, L.CompanyName, L.ListingURL, A.AssemblyID, C.Mobile , L.GSTNumber
                    FROM [listing].[Listing] L
                    INNER JOIN [listing].[Communication] C ON L.ListingID = C.ListingID
                    INNER JOIN [listing].[Address] A ON L.ListingID = A.ListingID
                    LEFT JOIN [dbo].[Packages] p ON l.PackageID = p.Id 
                    WHERE C.Mobile LIKE @PartialMobileNumber  ORDER BY CASE WHEN p.Price IS NOT NULL THEN 1 ELSE 2 END, CAST(p.Price AS INT) DESC ";

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
                                    },
                                    Mobilenumber = reader["Mobile"].ToString(),
                                    Gstnumber = reader["GSTNumber"].ToString()
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
                        SELECT L.ListingID, L.CompanyName, L.ListingURL, A.AssemblyID, C.Mobile, L.GSTNumber
                        FROM [listing].[Listing] L
                        INNER JOIN [listing].[Communication] C ON L.ListingID = C.ListingID
                        INNER JOIN [listing].[Address] A ON L.ListingID = A.ListingID
                        LEFT JOIN [dbo].[Packages] p ON l.PackageID = p.Id 
                        WHERE L.GSTNumber LIKE @GstNumber ORDER BY CASE WHEN p.Price IS NOT NULL THEN 1 ELSE 2 END, CAST(p.Price AS INT) DESC ";

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
                                    },
                                    Mobilenumber = reader["Mobile"].ToString(),
                                    Gstnumber = reader["GSTNumber"].ToString()
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
    }
}
