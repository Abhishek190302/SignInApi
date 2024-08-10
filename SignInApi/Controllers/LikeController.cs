using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly UserService _userService;
        private readonly ListingEnquiryService _listingEnquiryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public LikeController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor, CompanyDetailsRepository companydetailsRepository, ListingEnquiryService listingEnquiryService)
        {
            _connectionString = configuration.GetConnectionString("AuditTrail");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
            _listingEnquiryService = listingEnquiryService;
        }

        [HttpPost]
        [Route("Likes")]
        public async Task<ActionResult> Likes(LikeVM likeVM)
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
                            var like = await GetLikeDislikeByListingAndUserIdAsync(listing.Listingid, currentUserGuid);
                            if (like == null)
                            {
                                like = new LikeDislike
                                {
                                    ListingID = listing.Listingid,
                                    UserGuid = currentUserGuid,
                                    Mobile = userprofile.PhoneNumber,
                                    Email = userprofile.Email,
                                    VisitDate = DateTime.Now,
                                    VisitTime = DateTime.Now,
                                    LikeandDislike = true,
                                };
                                await AddLikeDislikeAsync(like);
                            }
                            else
                            {
                                like.LikeandDislike = !like.LikeandDislike;
                                await UpdateLikeDislikeAsync(like);
                            }
                            return Ok(like);
                        }
                        else
                        {
                            var like = await GetLikeDislikeByListingAndUserIdAsync(likeVM.companyID, currentUserGuid);
                            if (like == null)
                            {
                                like = new LikeDislike
                                {
                                    ListingID = likeVM.companyID,
                                    UserGuid = currentUserGuid,
                                    Mobile = userprofile.PhoneNumber,
                                    Email = userprofile.Email,
                                    VisitDate = DateTime.Now,
                                    VisitTime = DateTime.Now,
                                    LikeandDislike = true,
                                };
                                await AddLikeDislikeAsync(like);
                            }
                            else
                            {
                                like.LikeandDislike = !like.LikeandDislike;
                                await UpdateLikeDislikeAsync(like);
                            }
                            return Ok(like);

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

        private async Task<LikeDislike> GetLikeDislikeByListingAndUserIdAsync(int listingId, string userGuid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[LikeDislike] WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    return new LikeDislike
                    {
                        ListingID = row.Field<int>("ListingID"),
                        UserGuid = row.Field<string>("UserGuid"),
                        Mobile = row.Field<string>("Mobile"),
                        Email = row.Field<string>("Email"),
                        VisitDate = row.Field<DateTime>("VisitDate"),
                        VisitTime = row.Field<DateTime>("VisitTime"),
                        LikeandDislike = row.Field<bool>("Like")
                    };
                }
                return null;
            }
        }

        private async Task AddLikeDislikeAsync(LikeDislike like)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [audit].[LikeDislike] (ListingID, UserGuid, Mobile, Email, VisitDate, VisitTime, [Like]) " + "VALUES (@ListingID, @UserGuid, @Mobile, @Email, @VisitDate, @VisitTime, @Like)", conn);
                cmd.Parameters.AddWithValue("@ListingID", like.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", like.UserGuid);
                cmd.Parameters.AddWithValue("@Mobile", like.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", like.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VisitDate", like.VisitDate);
                cmd.Parameters.AddWithValue("@VisitTime", like.VisitTime);
                cmd.Parameters.AddWithValue("@Like", like.LikeandDislike);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateLikeDislikeAsync(LikeDislike like)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [audit].[LikeDislike] SET [Like] = @Like WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", like.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", like.UserGuid);
                cmd.Parameters.AddWithValue("@Like", like.LikeandDislike);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
