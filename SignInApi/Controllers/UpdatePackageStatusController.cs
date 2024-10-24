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
    public class UpdatePackageStatusController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UpdatePackageStatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("UpdatePackageStatus")]
        public async Task<IActionResult> UpdatePackageStatus(UpdatePackageStatus updatePackageStatus)
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("Select PackageStatus from [listing].[Listing] WHERE ListingID = @ListingId AND PackageID = @PackageID", conn);
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
                        // Get the PackageStatus
                        string packageStatus = dataTable.Rows[0]["PackageStatus"].ToString();

                        // Determine the package status message based on the status value
                        string statusMessage;
                        if (packageStatus == "0")
                        {
                            statusMessage = "Package Request";
                        }
                        else if (packageStatus == "1")
                        {
                            statusMessage = "Package Buy";
                        }
                        else
                        {
                            statusMessage = "Unknown Package Status";
                        }

                        // Return the appropriate response with the status message
                        return Ok(new
                        {
                            PackageStatus = packageStatus,
                            StatusMessage = statusMessage
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
