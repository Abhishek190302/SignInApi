using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        private readonly string _connectionAudit;
        public SuggestionController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("MimUser");
            _connectionAudit = configuration.GetConnectionString("AuditTrail");
        }

        [HttpPost]
        [Route("AddSuggestion")]
        public async Task<IActionResult> AddSuggestion(SuggestionRequest request)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Suggestion))
            {
                return BadRequest("All fields are compulsory.");
            }

            try
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
                            var userProfile = await GetProfileByOwnerGuid(currentUserGuid);

                            var suggestion = new Suggestion
                            {
                                OwnerGuid = currentUserGuid,
                                Mobile = applicationUser.PhoneNumber,
                                Email = applicationUser.Email,
                                Name = userProfile?.Name,
                                Title = request.Title,
                                SuggestionText = request.Suggestion,
                                Date = DateTime.Now,
                            };

                            await AddAsync(suggestion);

                            return Ok(new { Message = "Your Suggestion submitted successfully!", Suggestion = suggestion });
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
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<UserProfile> GetProfileByOwnerGuid(string ownerGuid)
        {
            UserProfile profile = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OwnerGuid, Name FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        await connection.OpenAsync();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            profile = new UserProfile
                            {
                                OwnerGuid = row["OwnerGuid"].ToString(),
                                Name = row["Name"].ToString()
                            };
                        }
                    }
                }
            }

            return profile;
        }


        private async Task AddAsync(Suggestion suggestion)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionAudit))
                {
                    string query = @"INSERT INTO [dbo].[Suggestions] (OwnerGuid,Date,Name,Email,Mobile,Title,Suggestion)
                             VALUES (@OwnerGuid,@Date, @Name,@Email, @Mobile,@Title, @SuggestionText)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OwnerGuid", suggestion.OwnerGuid);
                        command.Parameters.AddWithValue("@Date", suggestion.Date);
                        command.Parameters.AddWithValue("@Name", suggestion.Name);
                        command.Parameters.AddWithValue("@Email", suggestion.Email);
                        command.Parameters.AddWithValue("@Mobile", suggestion.Mobile);
                        command.Parameters.AddWithValue("@Title", suggestion.Title);
                        command.Parameters.AddWithValue("@SuggestionText", suggestion.SuggestionText);

                        connection.Open();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
