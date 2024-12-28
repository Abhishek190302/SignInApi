using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDeleteListingAccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserDeleteListingAccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("UserDeleteListing")]
        public async Task<IActionResult> UserDeleteListing([FromBody] UserDeleteListing userDeleteListing)
        {
            // Validate input
            if (userDeleteListing == null || userDeleteListing.ListingID <= 0)
            {
                return BadRequest("Invalid ListingID.");
            }

            string connectionString = _configuration.GetConnectionString("MimListing");
            const string mobileNumberQuery = "SELECT Mobile FROM [listing].[Communication] WHERE ListingID = @ListingID";
            const string deleteUserQuery = "DELETE FROM [MimUser_Api].[dbo].[AspNetUsers] WHERE PhoneNumber = @PhoneNumber";

            // Tables to clean up (order is critical for FK dependencies)
            string[] relatedTables = new[]
            {
                "[listing].[Communication]",
                "[listing].[Address]",
                "[listing].[Categories]",
                "[listing].[Specialisation]",
                "[listing].[WorkingHours]",
                "[listing].[PaymentMode]",
                "[dbo].[LogoImage]",
                "[dbo].[OwnerImage]",
                "[dbo].[GalleryImage]",
                "[dbo].[ClientDetail]",
                "[dbo].[CertificationDetail]",
                "[dbo].[BannerDetail]",
                "[dbo].[Keyword]",
                "[dbo].[SocialNetwork]",
                "[listing].[Listing]" // Main table deleted last
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Step 1: Retrieve mobile number for the ListingID
                            string mobileNumber = null;
                            using (SqlCommand getMobileCommand = new SqlCommand(mobileNumberQuery, conn, transaction))
                            {
                                getMobileCommand.Parameters.AddWithValue("@ListingID", userDeleteListing.ListingID);
                                var result = await getMobileCommand.ExecuteScalarAsync();
                                mobileNumber = result?.ToString();
                            }

                            // Step 2: Delete user from AspNetUsers table if a mobile number is found
                            if (!string.IsNullOrEmpty(mobileNumber))
                            {
                                using (SqlCommand deleteUserCommand = new SqlCommand(deleteUserQuery, conn, transaction))
                                {
                                    deleteUserCommand.Parameters.AddWithValue("@PhoneNumber", mobileNumber);
                                    await deleteUserCommand.ExecuteNonQueryAsync();
                                }
                            }

                            // Step 3: Delete records from related tables
                            foreach (var table in relatedTables)
                            {
                                string deleteQuery = $"DELETE FROM {table} WHERE ListingID = @ListingID";
                                using (SqlCommand command = new SqlCommand(deleteQuery, conn, transaction))
                                {
                                    command.Parameters.AddWithValue("@ListingID", userDeleteListing.ListingID);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            // Commit the transaction
                            await transaction.CommitAsync();
                            return Ok(new { Message = "Listing and associated data deleted successfully." });
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction on error
                            await transaction.RollbackAsync();
                            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred while deleting the listing.", Details = ex.Message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Database connection error.", Details = ex.Message });
            }
        }
    }
}
