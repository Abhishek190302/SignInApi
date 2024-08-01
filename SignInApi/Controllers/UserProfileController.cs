using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserProfileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("MimUser");
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            try
            {
                var user = _httpContextAccessor.HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    var userName = user.Identity.Name;

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        // Fetch user details
                        var userprofile = await GetUserByUserNameAsync(connection, userName);

                        if (user == null)
                        {
                            return NotFound("User not found.");
                        }

                        // Fetch user profile
                        var userProfile = await GetProfileByOwnerGuidAsync(connection, userprofile.Id);

                        var userProfileVM = new UserProfileVM
                        {
                            Email = userprofile.Email,
                            Phone = userprofile.PhoneNumber,
                            isVendor = userprofile.IsVendor,
                            Name = userProfile?.Name,
                            LastName = userProfile?.LastName,
                            ImgUrl = userProfile?.ImageUrl,
                            Gender = userProfile?.Gender
                        };

                        return Ok(userProfileVM);
                    }
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework here)
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        private async Task<ApplicationUserRequest> GetUserByUserNameAsync(SqlConnection connection, string userName)
        {
            var command = new SqlCommand("SELECT Id, Email, PhoneNumber, IsVendor FROM [dbo].[AspNetUsers] WHERE PhoneNumber = @UserName", connection);
            command.Parameters.AddWithValue("@UserName", userName);
            var dt = new DataTable();
            var da = new SqlDataAdapter(command);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new ApplicationUserRequest
                {
                    Id = row["Id"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    IsVendor = Convert.ToBoolean(row["IsVendor"])
                };
            }

            return null;
        }

        private async Task<UserProfileRequest> GetProfileByOwnerGuidAsync(SqlConnection connection, string ownerGuid)
        {
            var command = new SqlCommand("SELECT Name, LastName, ImageUrl, Gender FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid", connection);
            command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
            var dt = new DataTable();
            var da = new SqlDataAdapter(command);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new UserProfileRequest
                {
                    Name = row["Name"].ToString(),
                    LastName = row["LastName"].ToString(),
                    ImageUrl = row["ImageUrl"].ToString(),
                    Gender = row["Gender"].ToString()
                };
            }

            return null;
        }
    }
}
