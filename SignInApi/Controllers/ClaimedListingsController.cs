﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimedListingsController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public ClaimedListingsController(UserService userService, IConfiguration configuration, CompanyDetailsRepository companydetailsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
        }

        [HttpPost]
        [Route("Claimedlisting")]
        public IActionResult UpdateClaimedListing([FromBody] ClaimListingRequest request)
        {
            if (request == null || request.CompanyId <= 0)
            {
                return BadRequest(new { Message = "Invalid request data." });
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
                    return NotFound(new { Message = "Listing not found." });
                }

                int claimedListing = Convert.ToInt32(result);

                if (claimedListing == 1)
                {
                    SqlCommand updateCommand = new SqlCommand("UPDATE [listing].[Listing] SET ClaimedListing = 0,Status = 1 WHERE ListingID = @ListingId", connection);
                    updateCommand.Parameters.AddWithValue("@ListingId", request.CompanyId);
                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { Message = "Your listing has been successfully claimed." });
                    }
                    else
                    {
                        return StatusCode(500, new { Message = "Error updating listing." });
                    }
                }
                else
                {
                    return Ok(new { Message = "No update needed. ClaimedListing is already 0." });
                }
            }
        }


        //[HttpGet]
        //[Route("ClaimedUpdateListing")]
        //public async Task<IActionResult> ClaimedUpdateListing()
        //{
        //    string connectionString = _configuration.GetConnectionString("MimListing");

        //    var user = _httpContextAccessor.HttpContext.User;
        //    if (user.Identity.IsAuthenticated)
        //    {
        //        var userName = user.Identity.Name;

        //        var applicationUser = await _userService.GetUserByUserName(userName);
        //        if (applicationUser != null)
        //        {
        //            try
        //            {

        //                using (SqlConnection connection = new SqlConnection(connectionString))
        //                {
        //                    connection.Open();
        //                    SqlCommand selectCommand = new SqlCommand("SELECT ClaimedListing FROM [listing].[Listing] WHERE ListingID = @ListingId", connection);
        //                    selectCommand.Parameters.AddWithValue("@ListingId", listing.Listingid);

        //                    object result = selectCommand.ExecuteScalar();

        //                    if (result == null)
        //                    {
        //                        return NotFound(new { Message = "Listing not found." });
        //                    }

        //                    int claimedListing = Convert.ToInt32(result);

        //                    if (claimedListing == 1)
        //                    {
        //                        SqlCommand updateCommand = new SqlCommand("UPDATE [listing].[Listing] SET ClaimedListing = 0,Status = 1 WHERE ListingID = @ListingId", connection);
        //                        updateCommand.Parameters.AddWithValue("@ListingId", listing.Listingid);
        //                        int rowsAffected = updateCommand.ExecuteNonQuery();

        //                        if (rowsAffected > 0)
        //                        {
        //                            return Ok(new { Message = "Your listing has been successfully claimed." });
        //                        }
        //                        else
        //                        {
        //                            return StatusCode(500, new { Message = "Error updating listing." });
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return Ok(new { Message = "No update needed. ClaimedListing is already 0." });
        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                throw;
        //            }
        //        }
        //        return NotFound("User Not Found");
        //    }
        //    return Unauthorized();
        //}


        [HttpGet]
        [Route("ClaimedUpdateListing")]
        public async Task<IActionResult> ClaimedUpdateListing()
        {
            string connectionString = _configuration.GetConnectionString("MimListing");

            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Step 1: Retrieve ListingID based on mobile number from Communication table
                            SqlCommand getListingIdCommand = new SqlCommand("SELECT ListingID FROM [listing].[Communication] WHERE Mobile = @Mobile", connection);
                            getListingIdCommand.Parameters.AddWithValue("@Mobile", userName);
                            object listingIdResult = getListingIdCommand.ExecuteScalar();

                            if (listingIdResult == null)
                            {
                                return NotFound(new { Message = "Listing not found for the given mobile number." });
                            }

                            int listingId = Convert.ToInt32(listingIdResult);

                            // Step 2: Check the ClaimedListing status in the Listing table
                            SqlCommand selectCommand = new SqlCommand("SELECT ClaimedListing FROM [listing].[Listing] WHERE ListingID = @ListingId", connection);
                            selectCommand.Parameters.AddWithValue("@ListingId", listingId);

                            object result = selectCommand.ExecuteScalar();

                            if (result == null)
                            {
                                return NotFound(new { Message = "Listing not found." });
                            }

                            int claimedListing = Convert.ToInt32(result);

                            if (claimedListing == 1)
                            {
                                // Step 3: Update ClaimedListing and Status
                                SqlCommand updateCommand = new SqlCommand("UPDATE [listing].[Listing] SET ClaimedListing = 0, Status = 1 WHERE ListingID = @ListingId", connection);
                                updateCommand.Parameters.AddWithValue("@ListingId", listingId);
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    return Ok(new { Message = "Your listing has been successfully claimed." });
                                }
                                else
                                {
                                    return StatusCode(500, new { Message = "Error updating listing." });
                                }
                            }
                            else
                            {
                                return Ok(new { Message = "No update needed. ClaimedListing is already 0." });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
                    }
                }
                return NotFound("User Not Found");
            }
            return Unauthorized();
        }
    }
}
