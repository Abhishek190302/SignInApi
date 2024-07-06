using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Security.Claims;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly string _connectionString;

        public UserProfileController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimUser");
        }

        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfileAsync(string userName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Fetch user details
                    var user = await GetUserByUserNameAsync(connection, userName);

                    if (user == null)
                    {
                        return NotFound("User not found.");
                    }

                    // Fetch user profile
                    var userProfile = await GetProfileByOwnerGuidAsync(connection, user.Id);

                    var userProfileVM = new UserProfileVM
                    {
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        isVendor = user.IsVendor,
                        Name = userProfile?.Name,
                        LastName = userProfile?.LastName,
                        ImgUrl = userProfile?.ImageUrl
                    };

                    return Ok(userProfileVM);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework here)
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        private async Task<ApplicationUserRequest> GetUserByUserNameAsync(SqlConnection connection, string userName)
        {
            var command = new SqlCommand("SELECT Id, Email, PhoneNumber, IsVendor FROM [dbo].[AspNetUsers] WHERE UserName = @UserName", connection);
            command.Parameters.AddWithValue("@UserName", userName);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new ApplicationUserRequest
                    {
                        Id = reader["Id"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        IsVendor = Convert.ToBoolean(reader["IsVendor"])
                    };
                }
            }
            return null;
        }

        private async Task<UserProfileRequest> GetProfileByOwnerGuidAsync(SqlConnection connection, string ownerGuid)
        {
            var command = new SqlCommand("SELECT Name, LastName, ImageUrl, Gender FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid", connection);
            command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new UserProfileRequest
                    {
                        Name = reader["Name"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Gender = reader["Gender"].ToString()
                    };
                }
            }
            return null;
        }
    }
}
