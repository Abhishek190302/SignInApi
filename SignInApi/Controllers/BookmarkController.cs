using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly UserService _userService;
        private readonly ListingEnquiryService _listingEnquiryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CompanyDetailsRepository _companydetailsRepository;
        public BookmarkController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor, CompanyDetailsRepository companydetailsRepository, ListingEnquiryService listingEnquiryService)
        {
            _connectionString = configuration.GetConnectionString("AuditTrail");
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _companydetailsRepository = companydetailsRepository;
            _listingEnquiryService = listingEnquiryService;

        }

        [HttpPost]
        [Route("BookMarks")]
        public async Task<ActionResult> BookMarks(BookmarkVM bookmarkVM)
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
                            var bookmark = await GetBookmarkByListingAndUserIdAsync(bookmarkVM.companyID, currentUserGuid);
                            if (bookmark == null)
                            {
                                bookmark = new Bookmarks
                                {
                                    ListingID = bookmarkVM.companyID,
                                    UserGuid = currentUserGuid,
                                    Mobile = userprofile.PhoneNumber,
                                    Email = userprofile.Email,
                                    VisitDate = DateTime.Now,
                                    VisitTime = DateTime.Now,
                                    Bookmark = true,
                                };
                                await AddAsync(bookmark);
                            }
                            else
                            {
                                bookmark.Bookmark = !bookmark.Bookmark;
                                await UpdateAsync(bookmark);
                            }

                            return Ok(bookmark);
                        }
                        else
                        {
                            var bookmark = await GetBookmarkByListingAndUserIdAsync(bookmarkVM.companyID, currentUserGuid);
                            if (bookmark == null)
                            {
                                bookmark = new Bookmarks
                                {
                                    ListingID = bookmarkVM.companyID,
                                    UserGuid = currentUserGuid,
                                    Mobile = userprofile.PhoneNumber,
                                    Email = userprofile.Email,
                                    VisitDate = DateTime.Now,
                                    VisitTime = DateTime.Now,
                                    Bookmark = true,
                                };
                                await AddAsync(bookmark);
                            }
                            else
                            {
                                bookmark.Bookmark = !bookmark.Bookmark;
                                await UpdateAsync(bookmark);
                            }

                            return Ok(bookmark);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
                    }
                }
                else
                {
                    return NotFound("User not found.");
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        private async Task<Bookmarks> GetBookmarkByListingAndUserIdAsync(int listingId, string userGuid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [audit].[Bookmarks] WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);
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

        private async Task AddAsync(Bookmarks bookmark)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [audit].[Bookmarks] (ListingID, UserGuid, Mobile, Email, VisitDate, VisitTime, Bookmark) " + " VALUES (@ListingID, @UserGuid, @Mobile, @Email, @VisitDate, @VisitTime, @Bookmark)", conn);
                cmd.Parameters.AddWithValue("@ListingID", bookmark.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", bookmark.UserGuid);
                cmd.Parameters.AddWithValue("@Mobile", bookmark.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", bookmark.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VisitDate", bookmark.VisitDate);
                cmd.Parameters.AddWithValue("@VisitTime", bookmark.VisitTime);
                cmd.Parameters.AddWithValue("@Bookmark", bookmark.Bookmark);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateAsync(Bookmarks bookmark)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [audit].[Bookmarks] SET Bookmark = @Bookmark WHERE ListingID = @ListingID AND UserGuid = @UserGuid", conn);
                cmd.Parameters.AddWithValue("@ListingID", bookmark.ListingID);
                cmd.Parameters.AddWithValue("@UserGuid", bookmark.UserGuid);
                cmd.Parameters.AddWithValue("@Bookmark", bookmark.Bookmark);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
