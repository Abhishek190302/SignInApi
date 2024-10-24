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
    public class FreeSearchController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FreeSearchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("FreeSearch")]
        public async Task<IActionResult> FreeSearch(string? mobileNumber, string? ownerName, string? gstNumber, string? address)
        {
            if (string.IsNullOrEmpty(mobileNumber) && string.IsNullOrEmpty(ownerName) && string.IsNullOrEmpty(gstNumber) && string.IsNullOrEmpty(address))
                return BadRequest("At least one search parameter is required.");

            var result = await GetCompanyNameBySearch(mobileNumber, ownerName, gstNumber, address);
            if (result == null)
                return NotFound("No company found for the provided search criteria.");

            return Ok(result);
        }

        private async Task<List<CompanyFreeSearchResult>> GetCompanyNameBySearch(string? mobileNumber, string? ownerName, string? gstNumber, string? address)
        {
            List<CompanyFreeSearchResult> results = new List<CompanyFreeSearchResult>();
            string connectionString = _configuration.GetConnectionString("MimListing");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = "";
                
                if (!string.IsNullOrEmpty(mobileNumber))
                {
                    query += "SELECT L.ListingID, L.CompanyName, L.ListingURL FROM [listing].[Listing] L INNER JOIN [listing].[Communication] C ON L.ListingID = C.ListingID WHERE 1 = 1 AND C.Mobile = @MobileNumber";
                }

                if (!string.IsNullOrEmpty(ownerName))
                {
                    query += "SELECT L.ListingID, L.CompanyName, L.ListingURL FROM [listing].[Listing] L INNER JOIN [dbo].[OwnerImage] OI ON L.ListingID = OI.ListingID WHERE 1 = 1 AND OI.OwnerName LIKE @OwnerName";
                }

                if (!string.IsNullOrEmpty(gstNumber))
                {
                    query += "SELECT L.ListingID, L.CompanyName, L.ListingURL FROM [listing].[Listing] L WHERE 1 = 1 AND L.GSTNumber = @GstNumber";
                }

                if (!string.IsNullOrEmpty(address))
                {
                    query += "SELECT L.ListingID, L.CompanyName, L.ListingURL FROM [listing].[Listing] L INNER JOIN [listing].[Address] A ON L.ListingID = A.ListingID WHERE 1 = 1 AND A.LocalAddress LIKE @Address";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters to the command
                    if (!string.IsNullOrEmpty(mobileNumber))
                        cmd.Parameters.AddWithValue("@MobileNumber", mobileNumber);

                    if (!string.IsNullOrEmpty(ownerName))
                        cmd.Parameters.AddWithValue("@OwnerName", "%" + ownerName + "%");

                    if (!string.IsNullOrEmpty(gstNumber))
                        cmd.Parameters.AddWithValue("@GstNumber", gstNumber);

                    if (!string.IsNullOrEmpty(address))
                        cmd.Parameters.AddWithValue("@Address", "%" + address + "%");

                    // Use SqlDataAdapter to fill a DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int listingId = Convert.ToInt32(row["ListingID"]);

                        SubCategory subCategory = await GetFirstSubcategory(listingId);
                        int categoryId = subCategory?.Id ?? 0; // If null, default to 0
                        string categoryName = subCategory?.Name ?? string.Empty; // If null, default to an empty string
                        var result = new CompanyFreeSearchResult
                        {
                            ListingId = Convert.ToInt32(row["ListingID"]),
                            CompanyName = row["CompanyName"].ToString(),
                            ListingURL = row["ListingURL"].ToString(),
                            CategoryId = categoryId,  
                            CategoryName = categoryName
                        };
                        results.Add(result);  // Add the result to the list
                    }
                }
            }

            return results;  // Return the list of results
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
