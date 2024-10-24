using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordFreeSearchController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public KeywordFreeSearchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("KeywordFreesearch")]
        public IActionResult SearchCompanyByKeyword(string keyword)
        {
            try
            {
                var companies = GetCompaniesByKeyword(keyword);
                if (companies == null)
                {
                    return NotFound(new { message = "No companies found for the given keyword." });
                }
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        private async Task<List<CompanyFreeSearchResult>> GetCompaniesByKeyword(string keyword)
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            List<CompanyFreeSearchResult> companyResults = new List<CompanyFreeSearchResult>();

            // Update the SQL query to select more fields
            string query = @"
            SELECT l.ListingID, l.CompanyName, l.ListingURL, c.CategoryId, c.CategoryName
            FROM [listing].[Listing] l
            INNER JOIN [dbo].[Keyword] k ON l.ListingID = k.ListingID
            INNER JOIN [dbo].[Category] c ON l.CategoryID = c.CategoryId
            WHERE k.SeoKeyword = @Keyword
            AND k.ListingId <> 0";

            // Use SqlDataAdapter and DataTable to execute and store the result
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", keyword);

                    // Use SqlDataAdapter to fill a DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    // Loop through the DataTable to extract data and populate the CompanyFreeSearchResult model
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int listingId = Convert.ToInt32(row["ListingID"]);

                        SubCategory subCategory = await GetFirstSubcategory(listingId);
                        int categoryId = subCategory?.Id ?? 0; // If null, default to 0
                        string categoryName = subCategory?.Name ?? string.Empty; // If null, default to an empty string

                        CompanyFreeSearchResult result = new CompanyFreeSearchResult
                        {
                            ListingId = Convert.ToInt32(row["ListingID"]),
                            CompanyName = row["CompanyName"].ToString(),
                            ListingURL = row["ListingURL"].ToString(),
                            CategoryId = categoryId,
                            CategoryName = categoryName
                        };
                        companyResults.Add(result);
                    }
                }
            }

            return companyResults;
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
    }
}
