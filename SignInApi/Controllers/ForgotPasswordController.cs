using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        public ForgotPasswordController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("MimUser");
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(Forgotpassword request)
        {
            if (string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword))
            {
                return BadRequest("All fields are compulsory.");
            }

            var hasher = new PasswordHasher<Forgotpassword>();
            request.NewPassword = hasher.HashPassword(request, request.NewPassword);

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

                            SqlConnection con = new SqlConnection(_connectionString);                            
                            if (con.State == ConnectionState.Closed) { con.Open(); }
                            SqlCommand cmd1 = new SqlCommand("usp_updateForgotpassword", con);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@id", currentUserGuid);
                            cmd1.Parameters.AddWithValue("@password", request.NewPassword);
                            cmd1.Parameters.AddWithValue("@confirmpassword", request.ConfirmPassword);
                            cmd1.ExecuteNonQuery();
                            con.Close();
                            return Ok(new { Message = "Password Update Sucessfully...", Response = request });
                    
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
