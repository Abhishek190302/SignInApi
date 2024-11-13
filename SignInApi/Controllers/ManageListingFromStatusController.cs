using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageListingFromStatusController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public ManageListingFromStatusController(UserService userService,IConfiguration configuration,CompanyDetailsRepository companydetailsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
        }

        [HttpGet]
        [Route("GetManageListingFromStatus")]
        public async Task<IActionResult> GetManageListingFromStatus()
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
                        string currentUserGuid = applicationUser.Id.ToString();
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        ManageStatus listingStatus = new ManageStatus();
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            if(listing != null)
                            {
                                SqlCommand cmd = new SqlCommand("SELECT Status FROM [listing].[Listing] WHERE ListingID = @ListingId", connection);
                                cmd.Parameters.AddWithValue("@ListingId", listing.Listingid);
                                await connection.OpenAsync();
                                DataTable dt = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter(cmd);
                                da.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    DataRow row = dt.Rows[0];
                                    listingStatus.ListingId = listing.Listingid;
                                    listingStatus.Status = Convert.ToInt32(row["Status"]);
                                }
                                else
                                {
                                    return NotFound(new { message = "Listing not found" });
                                }
                            }
                            else
                            {
                                await connection.OpenAsync();
                                SqlCommand Command = new SqlCommand("SELECT ListingID FROM [listing].[Communication] WHERE Mobile = @Mobile", connection);
                                Command.Parameters.AddWithValue("@Mobile", userName);
                                object listingIdResult = Command.ExecuteScalar();

                                if (listingIdResult == null)
                                {
                                    return NotFound(new { Message = "Listing not found for the given mobile number." });
                                }

                                int listingId = Convert.ToInt32(listingIdResult);

                                // Step 2: Check the ClaimedListing status in the Listing table
                                SqlCommand selectCommand = new SqlCommand("SELECT ListingID,Status FROM [listing].[Listing] WHERE ListingID = @ListingId", connection);
                                selectCommand.Parameters.AddWithValue("@ListingId", listingId);
                                
                                DataTable dt = new DataTable();
                                SqlDataAdapter da = new SqlDataAdapter(selectCommand);
                                da.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    DataRow row = dt.Rows[0];
                                    listingStatus.ListingId = listingId;
                                    listingStatus.Status = Convert.ToInt32(row["Status"]);
                                }
                                else
                                {
                                    return NotFound(new { message = "Listing not found" });
                                }
                            }
                            
                        }

                        return Ok(listingStatus);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return NotFound("User Not Found");
            }
            return Unauthorized();
        }
    }
}
