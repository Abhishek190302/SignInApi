using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly UserService _userService;
        private readonly ListingEnquiryService _listingEnquiryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public SubscribeController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor, CompanyDetailsRepository companydetailsRepository, ListingEnquiryService listingEnquiryService)
        {
            _connectionString = configuration.GetConnectionString("AuditTrail");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
            _listingEnquiryService = listingEnquiryService;
        }

        [HttpPost]
        [Route("Subscribes")]
        public async Task<ActionResult> Subscribes(SubscribeVM subscribeVM)
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
                        var listing = await _companydetailsRepository.GetListingByOwnerIdAsync(currentUserGuid);
                        var userprofile = await _listingEnquiryService.GetUserByUserNameAsync(userName);
                        if (listing != null)
                        {
                            var subscribe = await GetSubscribeByListingAndUserIdAsync(subscribeVM.companyID, currentUserGuid);
                            if (subscribe == null)
                            {
                                subscribe = new Subscribes
                                {
                                    ListingID = subscribeVM.companyID,
                                    UserGuid = currentUserGuid,
                                    Mobile = userprofile.PhoneNumber,
                                    Email = userprofile.Email,
                                    VisitDate = DateTime.Now,
                                    VisitTime = DateTime.Now,
                                    Subscribe = true,
                                };
                                await AddSubscribeAsync(subscribe);
                            }
                            else
                            {
                                subscribe.Subscribe = !subscribe.Subscribe;
                                await UpdateSubscribeAsync(subscribe);
                            }
                            return Ok(subscribe);
                        }
                        else
                        {
                            var subscribe = await GetSubscribeByListingAndUserIdAsync(subscribeVM.companyID, currentUserGuid);
                            if (subscribe == null)
                            {
                                subscribe = new Subscribes
                                {
                                    ListingID = subscribeVM.companyID,
                                    UserGuid = currentUserGuid,
                                    Mobile = userprofile.PhoneNumber,
                                    Email = userprofile.Email,
                                    VisitDate = DateTime.Now,
                                    VisitTime = DateTime.Now,
                                    Subscribe = true,
                                };
                                await AddSubscribeAsync(subscribe);
                            }
                            else
                            {
                                subscribe.Subscribe = !subscribe.Subscribe;
                                await UpdateSubscribeAsync(subscribe);
                            }
                            return Ok(subscribe);
                        }
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


        private async Task<Subscribes> GetSubscribeByListingAndUserIdAsync(int listingId, string userGuid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[Subscribes] WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Subscribes
                    {
                        ListingID = row.Field<int>("ListingID"),
                        UserGuid = row.Field<string>("UserGuid"),
                        Mobile = row.Field<string>("Mobile"),
                        Email = row.Field<string>("Email"),
                        VisitDate = row.Field<DateTime>("VisitDate"),
                        VisitTime = row.Field<DateTime>("VisitTime"),
                        Subscribe = row.Field<bool>("Subscribe")
                    };
                }
                return null;
            }
        }

        private async Task AddSubscribeAsync(Subscribes subscribe)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [audit].[Subscribes] (ListingID, UserGuid, Mobile, Email, VisitDate, VisitTime, Subscribe) " + " VALUES (@ListingID, @UserGuid, @Mobile, @Email, @VisitDate, @VisitTime, @Subscribe)", conn);
                cmd.Parameters.AddWithValue("@ListingID", subscribe.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", subscribe.UserGuid);
                cmd.Parameters.AddWithValue("@Mobile", subscribe.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", subscribe.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VisitDate", subscribe.VisitDate);
                cmd.Parameters.AddWithValue("@VisitTime", subscribe.VisitTime);
                cmd.Parameters.AddWithValue("@Subscribe", subscribe.Subscribe);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateSubscribeAsync(Subscribes subscribe)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [audit].[Subscribes] SET Subscribe = @Subscribe WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", subscribe.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", subscribe.UserGuid);
                cmd.Parameters.AddWithValue("@Subscribe", subscribe.Subscribe);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
