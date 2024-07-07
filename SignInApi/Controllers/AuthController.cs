using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private readonly PasswordHasher<ApplicationUsers> _passwordHasher;
        public AuthController(IConfiguration configuration, TokenService tokenService)
        {
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<ApplicationUsers>();
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequests request)
        {
            if (string.IsNullOrEmpty(request.EmailOrMobile) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ErrorResponse { Message = "Email or Mobile and Password are required." });
            }

            var response = await SignIn(request.EmailOrMobile, request.Password, request.RememberMe);

            if (response.StatusCode == Constantss.Success)
            {
                var user = response.User; // Ensure this is an ApplicationUsers object
                var token = _tokenService.GenerateToken(user);
                return Ok(new { Token = token, RedirectToUrl = response.RedirectToUrl });

                //return Ok(response);
            }

            return Unauthorized(response);
        }

        private async Task<ErrorResponse> SignIn(string emailOrMobile, string password, bool rememberMe)
        {
            ErrorResponse errorResponse = new ErrorResponse();

            string connectionString = _configuration.GetConnectionString("MimUser");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[AspNetUsers] WHERE (PhoneNumber = @MobileOrEmail OR Email = @MobileOrEmail) AND IsRegistrationCompleted = 1", conn);
                cmd.Parameters.AddWithValue("@MobileOrEmail", emailOrMobile);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    errorResponse.Message = "Invalid Email ID or Password";
                    errorResponse.StatusCode = Constantss.Unauthorized;
                    return errorResponse;
                }

                DataRow row = dt.Rows[0];
                var usr = new ApplicationUsers
                {
                    Id = row["Id"].ToString(),
                    Email = row["Email"].ToString(),
                    PasswordHash = row["PasswordHash"].ToString(),
                    IsVendor = Convert.ToBoolean(row["IsVendor"])// Assuming you store the hash
                };

                bool canSignIn = await CanSignInAsync(usr);
                if (canSignIn)
                {
                    bool isPasswordValid = VerifyPasswordHash(password, usr); // Pass the usr object here
                    if (isPasswordValid)
                    {
                        var userProfile = await GetProfileByOwnerGuid(usr.Id);
                        errorResponse.RedirectToUrl = userProfile == null ? "/MyAccount/UserProfile" :
                            !userProfile.IsProfileCompleted ? "/MyAccount/ProfileInfo" : "/";
                        errorResponse.StatusCode = Constantss.Success;
                        errorResponse.User = usr; // Include user details in the response
                    }
                    else
                    {
                        errorResponse.Message = "Invalid Email ID or Password";
                        errorResponse.StatusCode = Constantss.Unauthorized;
                    }
                }
                else
                {
                    errorResponse.Message = "Your account is blocked";
                    errorResponse.StatusCode = Constantss.Unauthorized;
                }
            }

            return errorResponse;
        }

        private async Task<UserProfile> GetProfileByOwnerGuid(string ownerGuid)
        {
            UserProfile userProfile = null;
            string connectionString = _configuration.GetConnectionString("MimUser");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid", conn);
                cmd.Parameters.AddWithValue("@OwnerGuid", ownerGuid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    userProfile = new UserProfile
                    {
                        OwnerGuid = row["OwnerGuid"].ToString(),
                        IsProfileCompleted = Convert.ToBoolean(row["IsProfileCompleted"])
                    };
                }
            }

            return userProfile;
        }

        private async Task<bool> CanSignInAsync(ApplicationUsers user)
        {
            bool canSignIn = true;
            string connectionString = _configuration.GetConnectionString("MimUser");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT 0 FROM [dbo].[AspNetUsers] WHERE Id = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", user.Id);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    canSignIn = !Convert.ToBoolean(result);
                }
            }

            return canSignIn;
        }

        private bool VerifyPasswordHash(string password, ApplicationUsers user)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            // Log the verification result for debugging
            Console.WriteLine($"Verification Result: {verificationResult}");

            if (verificationResult == PasswordVerificationResult.Success || verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    // Rehash the password with the updated algorithm and save it
                    user.PasswordHash = _passwordHasher.HashPassword(user, password);
                    // Update the database with the new hash
                }
                return true;
            }

            return false;
        }
    }
}
