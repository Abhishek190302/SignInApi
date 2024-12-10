using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingAreaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WorkingAreaController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("SavePincodes")]
        public async Task<IActionResult> SavePincodes([FromBody] SavePincodesModel model)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userName = user.Identity.Name;

                var applicationUser = await _userService.GetUserByUserName(userName);
                if (applicationUser != null)
                {
                    try
                    {
                        string currentUserGuid = applicationUser.Id.ToString();

                        string connectionString = _configuration.GetConnectionString("MimListing");

                        if (model.ListingID == 0 || model.CountryID == 0 || model.StateID == 0 || model.CityID == 0 || model.AssemblyID == 0)
                        {
                            return BadRequest(new { success = false, message = "Invalid input parameters." });
                        }
                        if (model.SelectedPincodesContainer == null || !model.SelectedPincodesContainer.Any())
                        {
                            return BadRequest(new { success = false, message = "No pincodes selected." });
                        }

                        try
                        {
                            string pincodeString = string.Join(",", model.SelectedPincodesContainer);

                            int maxPincodeLimit = 0;
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                // Price retrieve karenge
                                string priceQuery = @"SELECT p.Price FROM [listing].[Listing] l INNER JOIN [dbo].[Packages] p ON l.PackageID = p.Id WHERE l.ListingID = @ListingID";
                                using (SqlCommand command = new SqlCommand(priceQuery, connection))
                                {
                                    command.Parameters.AddWithValue("@ListingID", model.ListingID);
                                    var price = command.ExecuteScalar();

                                    if (price != null)
                                    {
                                        int priceValue = Convert.ToInt32(price);
                                        if (priceValue == 3000) maxPincodeLimit = 5;
                                        else if (priceValue == 5000) maxPincodeLimit = 10;
                                        else if (priceValue == 7000) maxPincodeLimit = 15;
                                        else
                                        {
                                            return BadRequest(new { success = false, message = "Invalid package price." });
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new { success = false, message = "Listing not found in packages." });
                                    }
                                }
                            }

                            // Validate selected pincodes
                            if (model.SelectedPincodesContainer.Count > maxPincodeLimit)
                            {
                                return BadRequest(new { success = false, message = $"You can add only {maxPincodeLimit} pincodes for this package." });
                            }


                            // Save pincodes
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();


                                string query = @"INSERT INTO [dbo].[WorkingArea] 
                                (ListingID, OwnerGuid, CountryID, StateID, CityID, AssemblyID, PincodeID, CreateDate) 
                                VALUES (@ListingID, @OwnerGuid, @CountryID, @StateID, @CityID, @AssemblyID, @PincodeID, GETDATE())";

                                using (SqlCommand command = new SqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@ListingID", model.ListingID);
                                    command.Parameters.AddWithValue("@OwnerGuid", currentUserGuid);
                                    command.Parameters.AddWithValue("@CountryID", model.CountryID);
                                    command.Parameters.AddWithValue("@StateID", model.StateID);
                                    command.Parameters.AddWithValue("@CityID", model.CityID);
                                    command.Parameters.AddWithValue("@AssemblyID", model.AssemblyID);
                                    command.Parameters.AddWithValue("@PincodeID", pincodeString);
                                    command.ExecuteNonQuery();
                                }
                            }

                            return Ok(new { success = true, message = "WorkingArea created successfully!" });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { success = false, message = $"An error occurred: {ex.Message}" });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }
    }
}
