using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordshowfromstatusController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public KeywordshowfromstatusController(UserService userService, IConfiguration configuration, CompanyDetailsRepository companydetailsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
        }

        [HttpPost]
        [Route("GetKeywordshow")]
        public async Task<IActionResult> GetKeywordshow(KeywordVM keywordVM)
        {
            string connectionString = _configuration.GetConnectionString("MimListing");

            try
            {                
                ManageStatus listingStatus = new ManageStatus();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Status FROM [listing].[Listing] WHERE ListingID = @ListingId", connection);
                    cmd.Parameters.AddWithValue("@ListingId", keywordVM.companyID);
                    await connection.OpenAsync();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        listingStatus.ListingId = keywordVM.companyID;
                        listingStatus.Status = Convert.ToInt32(row["Status"]);
                    }
                    else
                    {
                        return NotFound(new { message = "Listing not found" });
                    }
                }

                return Ok(new { message = "Success", data = listingStatus });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
                
        }
    }
}
