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

        //[HttpPost]
        //[Route("UpdatePackageStatus")]
        //public async Task<IActionResult> UpdatePackageStatus(UpdatePackageStatus updatePackageStatus)
        //{
        //    string connectionString = _configuration.GetConnectionString("MimListing");
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("Select PackageStatus from [listing].[Listing] WHERE ListingID = @ListingId AND PackageID = @PackageID", conn);
        //        cmd.Parameters.AddWithValue("@ListingId", updatePackageStatus.companyID);
        //        cmd.Parameters.AddWithValue("@PackageID", updatePackageStatus.PackageID);

        //        try
        //        {
        //            await conn.OpenAsync();
        //            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
        //            DataTable dataTable = new DataTable();
        //            dataAdapter.Fill(dataTable);

        //            if (dataTable.Rows.Count > 0)
        //            {
        //                // Get the PackageStatus
        //                string packageStatus = dataTable.Rows[0]["PackageStatus"].ToString();

        //                // Determine the package status message based on the status value
        //                string statusMessage;
        //                if (packageStatus == "0")
        //                {
        //                    statusMessage = "Package Request";
        //                }
        //                else if (packageStatus == "1")
        //                {
        //                    statusMessage = "Package Buy";
        //                }
        //                else
        //                {
        //                    statusMessage = "Unknown Package Status";
        //                }

        //                // Return the appropriate response with the status message
        //                return Ok(new
        //                {
        //                    PackageStatus = packageStatus,
        //                    StatusMessage = statusMessage
        //                });
        //            }
        //            else
        //            {
        //                return NotFound(new { message = "Listing not found" });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        //        }
        //    }
        //}

        [HttpGet]
        [Route("UpdatePackageStatus")]
        public async Task<IActionResult> UpdatePackageStatus()
        {
            string connectionString = _configuration.GetConnectionString("MimListing");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT L.ListingID, L.PackageID, L.PackageStatus FROM [listing].[Listing] L JOIN [dbo].[Packages] P ON L.PackageID = P.Id", conn);

                try
                {
                    await conn.OpenAsync();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        // Create a list to store each listing's status info
                        var packageStatuses = new List<object>();

                        // Loop through each row in the DataTable
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string packageStatus = row["PackageStatus"].ToString();

                            // Determine the package status message based on the status value
                            string statusMessage = packageStatus switch
                            {
                                "0" => "Package Request",
                                "1" => "Package Buy",
                                _ => "Unknown Package Status"
                            };

                            // Add the current listing's status info to the list
                            packageStatuses.Add(new
                            {
                                ListingID = row["ListingID"],
                                PackageID = row["PackageID"],
                                PackageStatus = packageStatus,
                                StatusMessage = statusMessage
                            });
                        }

                        // Return the full list of package statuses
                        return Ok(packageStatuses);
                    }
                    else
                    {
                        return NotFound(new { message = "No listings found" });
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
