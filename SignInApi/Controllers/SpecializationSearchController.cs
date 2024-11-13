using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationSearchController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SpecializationSearchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("SpecializationSearchCompany")]
        public async Task<IActionResult> SpecializationSearchCompany(string searchQuery)
        {
            try
            {
                List<CompanySearchResult> companies = new List<CompanySearchResult>();

                // Parse the search query to extract keyword, location, and specialization
                (string keyword, string location, string specialization) = ParseSearchQuery(searchQuery);

                string connectionString = _configuration.GetConnectionString("MimListing");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT l.ListingID, l.CompanyName, l.ListingURL, c.Name as City, loc.Name as Locality, k.KeywordID, k.SeoKeyword
                FROM [dbo].[Keyword] k
                INNER JOIN [listing].[Address] a ON k.ListingID = a.ListingID
                INNER JOIN [listing].[Listing] l ON a.ListingID = l.ListingID
                INNER JOIN [MimShared].[shared].[City] c ON a.City = c.CityID
                INNER JOIN [MimShared].[dbo].[Location] loc ON a.AssemblyID = loc.Id
                INNER JOIN [listing].[Specialisation] s ON l.ListingID = s.ListingID
                WHERE k.SeoKeyword LIKE '%' + @Keyword + '%'
                AND (c.Name LIKE '%' + @Location + '%' OR loc.Name LIKE '%' + @Location + '%')
                AND (
                        (@Specialization = 'Church' AND s.Church = 1) OR
                        (@Specialization = 'Salons' AND s.Salons = 1) OR
                        (@Specialization = 'Banks' AND s.Banks = 1)
                        
                    );";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Keyword", keyword);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@Specialization", specialization); // Add specialization parameter

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
                            ListingID = Convert.ToInt32(row["ListingID"]),
                            CompanyName = row["CompanyName"].ToString(),
                            ListingURL = row["ListingURL"].ToString(),
                            City = row["City"].ToString(),
                            Locality = row["Locality"].ToString(),
                            KeywordID = Convert.ToInt32(row["KeywordID"]),
                            Keyword = row["SeoKeyword"].ToString(),
                            CategoryId = subCategory != null ? subCategory.Id : 0,
                            Category = subCategory != null ? subCategory.Name : string.Empty,
                            Specialization = specialization // Set specialization
                        };
                        companies.Add(result);
                    }
                }
                return Ok(companies);
            }
            catch(Exception ex)
            {
                throw;
            }
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

        private (string specialization, string keyword, string location) ParseSearchQuery(string searchQuery)
        {
            string specialization = string.Empty;
            string keyword = string.Empty;
            string location = string.Empty;

            // Look for "near by" to identify the location part
            var locationSplit = searchQuery.Split(new[] { " near by " }, StringSplitOptions.None);

            if (locationSplit.Length > 1)
            {
                location = locationSplit[1].Trim(); // Location is after "near by"
            }

            // Remaining text before "near by" may contain specialization and keyword
            var remainingText = locationSplit[0].Trim();

            // Assuming specialization is the first word, and the rest is the keyword
            var remainingParts = remainingText.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (remainingParts.Length > 0)
            {
                specialization = remainingParts[0]; // First word as specialization
            }

            if (remainingParts.Length > 1)
            {
                keyword = remainingParts[1]; // Rest of the text as keyword
            }

            return (keyword, location, specialization);
        }
    }
}
