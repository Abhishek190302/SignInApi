using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimedListingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ClaimedListingsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Claimedlisting")]
        public IActionResult UpdateClaimedListing([FromBody] ClaimListingRequest request)
        {
            if (request == null || request.CompanyId <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            string connectionString = _configuration.GetConnectionString("MimListing");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("SELECT ClaimedListing FROM [listing].[Listing] WHERE ListingID = @ListingId", connection);
                selectCommand.Parameters.AddWithValue("@ListingId", request.CompanyId);

                object result = selectCommand.ExecuteScalar();

                if (result == null)
                {
                    return NotFound("Listing not found.");
                }

                int claimedListing = Convert.ToInt32(result);

                if (claimedListing == 1)
                {
                    SqlCommand updateCommand = new SqlCommand("UPDATE [listing].[Listing] SET ClaimedListing = 0,Status = 1 WHERE ListingID = @ListingId", connection);
                    updateCommand.Parameters.AddWithValue("@ListingId", request.CompanyId);
                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Your Listing is claim successfully.");
                    }
                    else
                    {
                        return StatusCode(500, "Error updating listing.");
                    }
                }
                else
                {
                    return Ok("No update needed. ClaimedListing is already 0.");
                }
            }
        }
    }
}
