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
    public class PackageBuyController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public PackageBuyController(UserService userService, IHttpContextAccessor httpContextAccessor, CompanyDetailsRepository companyDetailsRepository, IConfiguration configuration)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companyDetailsRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("UpdatePackageStatus")]
        public async Task<IActionResult> UpdatePackageStatus(UpdatePackageStatus updatePackageStatus)
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE [listing].[Listing] SET PackageBuyDate = GETDATE(), PackageStatus = '0', PackageID = @PackageID " +
                    "OUTPUT INSERTED.CompanyName WHERE ListingID = @ListingId", conn);
                    cmd.Parameters.AddWithValue("@ListingId", updatePackageStatus.companyID);
                    cmd.Parameters.AddWithValue("@PackageID", updatePackageStatus.PackageID);

                try
                {
                    await conn.OpenAsync();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        string companyName = dataTable.Rows[0]["CompanyName"].ToString();

                        return Ok(new
                        {
                            message = "Package Buy Successfully....",
                            companyName = companyName
                        });
                    }
                    else
                    {
                        return NotFound(new { message = "Listing not found" });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Internal server error", error = ex.Message });
                }
            }
        }
    }

}
