using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditRegisterController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        public EditRegisterController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("MimUser");
        }

        [HttpPost]
        [Route("EditProfileRegister")]
        public async Task<IActionResult> EditProfileRegister(EditRegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.MobileNumber) || string.IsNullOrEmpty(request.Email))
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
                            using (var connection = new SqlConnection(_connectionString))
                            {
                                using (var command = new SqlCommand("Update [dbo].[AspNetUsers] Set Email=@Email,PhoneNUmber=@PhoneNumber Where Id=@Id;", connection))
                                {
                                    command.Parameters.AddWithValue("@Id", currentUserGuid);
                                    command.Parameters.AddWithValue("@PhoneNumber", request.MobileNumber);
                                    command.Parameters.AddWithValue("@Email", request.Email);
                                    connection.Open();
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            var response = new
                            {
                                Message = "Update Successfully Mobile and EmailId...",
                                Email = request.Email,
                                PhoneNumber = request.MobileNumber
                            };

                            return Ok(response);
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
    }
}
