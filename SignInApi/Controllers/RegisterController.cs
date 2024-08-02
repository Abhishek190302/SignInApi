using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using static System.Net.WebRequestMethods;
using System.Data.SqlClient;
using System.Data;
using Twilio.TwiML.Messaging;
using Microsoft.AspNetCore.Cors;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        public RegisterController(IConfiguration configuration, TokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("RegisterPanel")]
        public IActionResult RegisterPanel([FromBody] RegisterRequest request)
        {
            bool emailconfirmed = true, phonenumberconfirmed = true, twofactorenable = false, lockoutenable = true, isregistrationcompleted = true;
            ErrorResponse errorResponse = new ErrorResponse();
            // Hash the password
            var hasher = new PasswordHasher<RegisterRequest>();
            request.Password = hasher.HashPassword(request, request.Password);

            var userId = Guid.NewGuid();
            var securityStamp = GenerateSecurityStamp();
            var concurrencyStamp = Guid.NewGuid().ToString();

            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("c4NjY4NTMyMTMiLCJhdWQiOiIxOTg0OTkz"))
            {

                if (IsMobileNumberExists(request.Mobile))
                {
                    errorResponse.Message = "Mobile number already exists.";
                    errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                    return BadRequest(errorResponse);
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser")))
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        using (SqlCommand cmd = new SqlCommand("usp_RegisteruserConsumer", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.Parameters.AddWithValue("@vendortype", request.Vendortype);
                            cmd.Parameters.AddWithValue("@username", request.Mobile);
                            cmd.Parameters.AddWithValue("@NormalizedUserName", request.Mobile);
                            cmd.Parameters.AddWithValue("@email", request.Email);
                            cmd.Parameters.AddWithValue("@NormalizedEmail", request.Email);
                            cmd.Parameters.AddWithValue("@mobile", request.Mobile);
                            cmd.Parameters.AddWithValue("@password", request.Password);
                            cmd.Parameters.AddWithValue("@securitystamp", securityStamp);
                            cmd.Parameters.AddWithValue("@concurrencystamp", concurrencyStamp);
                            cmd.Parameters.AddWithValue("@confirmpassword", request.Confirmpassword);
                            cmd.Parameters.AddWithValue("@Emailconfirmed", emailconfirmed);
                            cmd.Parameters.AddWithValue("@Phonenumberconfirmed", phonenumberconfirmed);
                            cmd.Parameters.AddWithValue("@Twofactorenable", twofactorenable);
                            cmd.Parameters.AddWithValue("@Lockoutenable", lockoutenable);
                            cmd.Parameters.AddWithValue("@Isregistrationcompleted", isregistrationcompleted);
                            cmd.ExecuteNonQuery();
                        }



                        SqlCommand cmduser = new SqlCommand("SELECT * FROM [dbo].[AspNetUsers]  WHERE (PhoneNumber = @MobileOrEmail OR Email = @MobileOrEmail) AND IsRegistrationCompleted = 1", con);
                        cmduser.Parameters.AddWithValue("@MobileOrEmail", request.Mobile);
                        SqlDataAdapter da = new SqlDataAdapter(cmduser);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            errorResponse.Message = "Invalid Email ID or Password";
                            errorResponse.StatusCode = Constantss.Unauthorized;
                            return BadRequest("Invalid Email ID or Password");
                        }

                        DataRow row = dt.Rows[0];
                        var usr = new ApplicationUsers
                        {
                            Id = row["Id"].ToString(),
                            Email = row["Email"].ToString(),
                            phone = row["PhoneNumber"].ToString(),
                            PasswordHash = row["PasswordHash"].ToString(),
                            IsVendor = Convert.ToBoolean(row["IsVendor"])// Assuming you store the hash
                        };
                        var token = _tokenService.GenerateToken(usr);
                        return Ok(new { Message = "User registered consumer account successfully...", Response = request, Token = token });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new RegisterResponse { Message = "User not registered..." });
                }
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized("Unauthorized: Invalid token.");
            }
        }


        [HttpPost]
        [Route("RegisterPanelBusiness")]
        public IActionResult RegisterPanelBusiness([FromBody] RegisterRequestBusiness request)
        {
            bool emailconfirmed = true, phonenumberconfirmed = true, twofactorenable = false, lockoutenable = true, isregistrationcompleted = true;
            ErrorResponse errorResponse = new ErrorResponse();
            // Hash the password
            var hasher = new PasswordHasher<RegisterRequestBusiness>();
            request.Password = hasher.HashPassword(request, request.Password);

            var userId = Guid.NewGuid();
            var securityStamp = GenerateSecurityStamp();
            var concurrencyStamp = Guid.NewGuid().ToString();

            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("c4NjY4NTMyMTMiLCJhdWQiOiIxOTg0OTgH"))
            {

                if (IsMobileNumberExists(request.Mobile))
                {
                    errorResponse.Message = "Mobile number already exists.";
                    errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                    return BadRequest(errorResponse);
                }


                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("usp_RegisteruserBusiness", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.Parameters.AddWithValue("@vendortype", request.Vendortype);
                cmd.Parameters.AddWithValue("@email", request.Email);
                cmd.Parameters.AddWithValue("@username", request.Mobile);
                cmd.Parameters.AddWithValue("@NormalizedUserName", request.Mobile);
                cmd.Parameters.AddWithValue("@NormalizedEmail", request.Email);
                cmd.Parameters.AddWithValue("@mobile", request.Mobile);
                cmd.Parameters.AddWithValue("@password", request.Password);
                cmd.Parameters.AddWithValue("@securitystamp", securityStamp);
                cmd.Parameters.AddWithValue("@concurrencystamp", concurrencyStamp);
                cmd.Parameters.AddWithValue("@confirmpassword", request.Confirmpassword);
                cmd.Parameters.AddWithValue("@businesscategory", request.Businesscategory);
                cmd.Parameters.AddWithValue("@Emailconfirmed", emailconfirmed);
                cmd.Parameters.AddWithValue("@Phonenumberconfirmed", phonenumberconfirmed);
                cmd.Parameters.AddWithValue("@Twofactorenable", twofactorenable);
                cmd.Parameters.AddWithValue("@Lockoutenable", lockoutenable);
                cmd.Parameters.AddWithValue("@Isregistrationcompleted", isregistrationcompleted);



                cmd.ExecuteNonQuery();
                con.Close();

                SqlCommand cmdbussi = new SqlCommand("SELECT * FROM [dbo].[AspNetUsers]  WHERE (PhoneNumber = @MobileOrEmail OR Email = @MobileOrEmail) AND IsRegistrationCompleted = 1", con);
                cmdbussi.Parameters.AddWithValue("@MobileOrEmail", request.Mobile);
                SqlDataAdapter da = new SqlDataAdapter(cmdbussi);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    errorResponse.Message = "Invalid Email ID or Password";
                    errorResponse.StatusCode = Constantss.Unauthorized;
                    return BadRequest("Invalid Email ID or Password");
                }

                DataRow row = dt.Rows[0];
                var usr = new ApplicationUsers
                {
                    Id = row["Id"].ToString(),
                    Email = row["Email"].ToString(),
                    phone = row["PhoneNumber"].ToString(),
                    PasswordHash = row["PasswordHash"].ToString(),
                    IsVendor = Convert.ToBoolean(row["IsVendor"])// Assuming you store the hash
                };
                var token = _tokenService.GenerateToken(usr);
                return Ok(new { Message = "User Register business account successfully...", Response = request, Token = token });                    
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest(Response.WriteAsync("Unauthorized: Invalid token."));
            }
        }

        public static string GenerateRandomid()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[16];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new Guid(randomBytes).ToString();
            }
        }

        private bool IsMobileNumberExists(string mobileNumber)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser")))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[AspNetUsers] WHERE PhoneNumber = @PhoneNumber", con);
                cmd.Parameters.AddWithValue("@PhoneNumber", mobileNumber);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private string GenerateSecurityStamp()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[32];
            using (var random = new RNGCryptoServiceProvider())
            {
                var data = new byte[4 * stringChars.Length];
                random.GetBytes(data);
                for (int i = 0; i < stringChars.Length; i++)
                {
                    var rnd = BitConverter.ToUInt32(data, i * 4);
                    var idx = rnd % chars.Length;
                    stringChars[i] = chars[(int)idx];
                }
            }
            return new string(stringChars);
        }
    }
}
