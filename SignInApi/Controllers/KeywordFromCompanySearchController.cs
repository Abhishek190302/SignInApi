using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordFromCompanySearchController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public KeywordFromCompanySearchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("SearchCompany")]
        public async Task<IActionResult> SearchCompany(string searchQuery)
        {
            List<CompanySearchResult> companies = new List<CompanySearchResult>();

            // Parse the search query to extract keyword and location
            (string keyword, string location) = ParseSearchQuery(searchQuery);

            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT l.ListingID,l.CompanyName,l.ListingURL, c.Name as City, loc.Name as Locality, k.KeywordID, k.SeoKeyword
                FROM [dbo].[Keyword] k
                INNER JOIN [listing].[Address] a ON k.ListingID = a.ListingID
                INNER JOIN [listing].[Listing] l ON a.ListingID = l.ListingID
                INNER JOIN [MimShared].[shared].[City] c ON a.City = c.CityID
                INNER JOIN [MimShared].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                WHERE k.SeoKeyword LIKE '%' + @Keyword + '%'
                AND (c.Name LIKE '%' + @Location + '%' OR loc.Name LIKE '%' + @Location + '%')";


                //string query = @"
                //SELECT l.ListingID,l.CompanyName,l.ListingURL, c.Name as City, loc.Name as Locality, k.KeywordID, k.SeoKeyword
                //FROM [dbo].[Keyword] k
                //INNER JOIN [listing].[Address] a ON k.ListingID = a.ListingID
                //INNER JOIN [listing].[Listing] l ON a.ListingID = l.ListingID
                //INNER JOIN [MimShared_Api].[shared].[City] c ON a.City = c.CityID
                //INNER JOIN [MimShared_Api].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                //WHERE k.SeoKeyword LIKE '%' + @Keyword + '%'
                //AND (c.Name LIKE '%' + @Location + '%' OR loc.Name LIKE '%' + @Location + '%')";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Keyword", keyword);
                cmd.Parameters.AddWithValue("@Location", location);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                await connection.OpenAsync();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    int listingId = Convert.ToInt32(row["ListingID"]);

                    // Fetch the first category for the listing
                    var subCategory = await GetFirstSubcategory(listingId);

                    CompanySearchResult result = new CompanySearchResult
                    {
                        ListingID = Convert.ToInt32( row["ListingID"]),
                        CompanyName = row["CompanyName"].ToString(),
                        ListingURL = row["ListingURL"].ToString(),
                        City = row["City"].ToString(),
                        Locality = row["Locality"].ToString(),
                        KeywordID = Convert.ToInt32(row["KeywordID"]),
                        Keyword = row["SeoKeyword"].ToString(),
                        CategoryId = subCategory != null ? subCategory.Id : 0, 
                        Category = subCategory != null ? subCategory.Name : string.Empty
                    };
                    companies.Add(result);
                }
            }

            return Ok(companies);
        }

        private async Task<SubCategory> GetFirstSubcategory(int listingId)
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 SecondCategoryID FROM [listing].[Categories] WHERE ListingID = @ListingId", conn);
                cmd.Parameters.AddWithValue("@ListingId", listingId);
                await conn.OpenAsync();
                int subCategoryId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                // Get the subcategory name using the retrieved SubCategoryId
                string subCategoryName = await GetSecondCategoryById(subCategoryId);

                if (!string.IsNullOrEmpty(subCategoryName))
                {
                    return new SubCategory
                    {
                        Id = subCategoryId,
                        Name = subCategoryName
                    };
                }
            }

            return null;
        }

        private async Task<string> GetSecondCategoryById(int categoryId)
        {
            string connectionString = _configuration.GetConnectionString("MimCategories");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Name FROM [cat].[SecondCategory] WHERE SecondCategoryID = @SecondCategoryID", conn);
                cmd.Parameters.AddWithValue("@SecondCategoryID", categoryId);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        // Helper method to parse the search query
        //private (string, string) ParseSearchQuery(string searchQuery)
        //{
        //    // Split the search query into words
        //    var words = searchQuery.Split(' ');

        //    // Initialize keyword and location variables
        //    string keyword = "";
        //    string location = "";

        //    // Check for "near by" or "near" to extract the location
        //    if (searchQuery.Contains("near by", StringComparison.OrdinalIgnoreCase) || searchQuery.Contains("near", StringComparison.OrdinalIgnoreCase))
        //    {
        //        int nearIndex = Array.FindIndex(words, w => w.Equals("near", StringComparison.OrdinalIgnoreCase) || w.Equals("near by", StringComparison.OrdinalIgnoreCase));
        //        if (nearIndex != -1 && nearIndex + 1 < words.Length)
        //        {
        //            // Extract the location after "near" or "near by"
        //            location = string.Join(" ", words.Skip(nearIndex + 2));
        //            // Extract the keyword before "near" or "near by"
        //            keyword = string.Join(" ", words.Take(nearIndex));
        //        }
        //    }
        //    else
        //    {
        //        // If no "near" is found, treat the whole string as a keyword
        //        keyword = searchQuery;
        //    }

        //    location = location.Replace("near by", "", StringComparison.OrdinalIgnoreCase)
        //           .Replace("near", "", StringComparison.OrdinalIgnoreCase)
        //           .Trim();

        //    return (keyword.Trim(), location.Trim());
        //}

        //private (string, string) ParseSearchQuery(string searchQuery)
        //{
        //    // Split the search query into words
        //    var words = searchQuery.Split(' ');

        //    // Initialize keyword and location variables
        //    string keyword = "";
        //    string location = "";

        //    // Define possible location-related terms
        //    string[] locationTerms = new[] { "near", "near by", "at", "in", "near me" };

        //    // Check if the search query contains any of the location-related terms
        //    var foundTerm = words.FirstOrDefault(w => locationTerms.Contains(w, StringComparer.OrdinalIgnoreCase));

        //    if (!string.IsNullOrEmpty(foundTerm))
        //    {
        //        int termIndex = Array.FindIndex(words, w => w.Equals(foundTerm, StringComparison.OrdinalIgnoreCase));

        //        // Extract the keyword before the location term
        //        keyword = string.Join(" ", words.Take(termIndex));

        //        // Extract the location after the location term
        //        if (foundTerm.Equals("near me", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Special case for "near me"
        //            location = ""; // Use user's current location or leave it blank
        //        }
        //        else if (termIndex + 1 < words.Length)
        //        {
        //            location = string.Join(" ", words.Skip(termIndex + 1));
        //        }
        //    }
        //    else
        //    {
        //        // If no location term is found, treat the whole string as a keyword
        //        keyword = searchQuery;
        //    }

        //    // Clean up the location string by removing any location-related terms
        //    location = location.Replace("near by", "", StringComparison.OrdinalIgnoreCase)
        //                       .Replace("near", "", StringComparison.OrdinalIgnoreCase)
        //                       .Replace("at", "", StringComparison.OrdinalIgnoreCase)
        //                       .Replace("in", "", StringComparison.OrdinalIgnoreCase)
        //                       .Trim();

        //    return (keyword.Trim(), location.Trim());
        //}

        private (string, string) ParseSearchQuery(string searchQuery)
        {
            // Define common phrases to be removed
            string[] commonPhrases = { "near", "near by", "at", "in", "near me" };

            // Normalize the search query to lower case for consistent comparison
            string normalizedQuery = searchQuery.ToLower();

            // Remove common phrases
            foreach (var phrase in commonPhrases)
            {
                normalizedQuery = normalizedQuery.Replace(phrase, "").Trim();
            }

            // Split the search query into keyword and location parts
            string[] parts = normalizedQuery.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            // Assume the last word(s) refer to the location, and the rest is the keyword
            string location = parts.LastOrDefault() ?? string.Empty;
            string keyword = parts.FirstOrDefault() ?? string.Empty;
            //string keyword = string.Join(" ", parts.Take(parts.Length - 1));

            return (keyword.Trim(), location.Trim());
        }
    }
}
