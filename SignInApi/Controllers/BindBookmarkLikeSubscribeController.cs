using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindBookmarkLikeSubscribeController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public BindBookmarkLikeSubscribeController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor, CompanyDetailsRepository companydetailsRepository)
        {
            _connectionString = configuration.GetConnectionString("AuditTrail");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;

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
                        if (listing != null)
                        {
                            var subscribe = await GetSubscribeByListingAndUserIdAsync(listing.Listingid, currentUserGuid);
                            return Ok(subscribe);
                        }
                        else
                        {
                            var subscribe = await GetSubscribeByListingAndUserIdAsync(subscribeVM.companyID ,currentUserGuid);
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


        [HttpPost]
        [Route("LikeDislike")]
        public async Task<ActionResult> LikeDislike(LikeVM likeVM)
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
                        if (listing != null)
                        {
                            var likedislike = await GetLikeDislikeByListingAndUserIdAsync(listing.Listingid, currentUserGuid);
                            return Ok(likedislike);
                        }
                        else
                        {
                            var likedislike = await GetLikeDislikeByListingAndUserIdAsync(likeVM.companyID,currentUserGuid);
                            return Ok(likedislike);
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


        [HttpPost]
        [Route("Bookmark")]
        public async Task<ActionResult> Bookmark(BookmarkVM bookmarkVM)
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
                        if (listing != null)
                        {
                            var bookmark = await GetBookmarkByListingAndUserIdAsync(listing.Listingid, currentUserGuid);
                            return Ok(bookmark);
                        }
                        else
                        {
                            var bookmark = await GetBookmarkByListingAndUserIdAsync(bookmarkVM.companyID,currentUserGuid);
                            return Ok(bookmark);
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



        private async Task<Bookmarks> GetBookmarkByListingAndUserIdAsync(int listingId, string Userid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[Bookmarks] WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                cmd.Parameters.AddWithValue("@UserGuid", Userid);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    return new Bookmarks
                    {
                        ListingID = row.Field<int>("ListingID"),
                        UserGuid = row.Field<string>("UserGuid"),
                        Mobile = row.Field<string>("Mobile"),
                        Email = row.Field<string>("Email"),
                        VisitDate = row.Field<DateTime>("VisitDate"),
                        VisitTime = row.Field<DateTime>("VisitTime"),
                        Bookmark = row.Field<bool>("Bookmark")
                    };
                }

                return null;
            }
        }

        private async Task<LikeDislike> GetLikeDislikeByListingAndUserIdAsync(int listingId, string Userid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[LikeDislike] WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                cmd.Parameters.AddWithValue("@UserGuid", Userid);
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

        private async Task<Subscribes> GetSubscribeByListingAndUserIdAsync(int listingId, string Userid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[Subscribes] WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                cmd.Parameters.AddWithValue("@UserGuid", Userid);
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
    }
}
