using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingPackageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ListingPackageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetListingPackage")]
        public async Task<IActionResult> GetListingPackage()
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            List<ListingPackage> listingPackages = new List<ListingPackage>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Packages]", conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ListingPackage listingPackage = new ListingPackage
                                {
                                    id = Convert.ToInt32(reader["Id"]),
                                    PackageTitle = reader["PackageTitle"].ToString(),
                                    Price = reader["Price"].ToString(),
                                    PackageImagepath = reader["PackageImagePath"].ToString(),
                                    PackageStatus = reader["PackageStatus"].ToString(),
                                    PackageDescription = reader["PackageDescription"].ToString(),
                                    PackageCreatedDate = Convert.ToDateTime(reader["PackageCreatedDate"]).ToString("dd-MM-yyyy hh:mm:ss"),
                                    PackageUpdatedDate = Convert.ToDateTime(reader["PackageUpdatedDate"]).ToString("dd-MM-yyyy hh:mm:ss")
                                };

                                listingPackages.Add(listingPackage);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            var response = new
            {
                ListingPackages = listingPackages
            };

            return Ok(response);
        }
    }
}
