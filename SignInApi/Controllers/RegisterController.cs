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
        
        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("RegisterPanel")]
        public IActionResult RegisterPanel([FromBody] RegisterRequest request)
        {
            bool emailconfirmed = true,phonenumberconfirmed = true,twofactorenable = false,lockoutenable = true,isregistrationcompleted=true;

            // Hash the password
            var hasher = new PasswordHasher<RegisterRequest>();
            request.Password = hasher.HashPassword(request, request.Password);

            var userId = Guid.NewGuid();
            var securityStamp = GenerateSecurityStamp();
            var concurrencyStamp = Guid.NewGuid().ToString();

            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("c4NjY4NTMyMTMiLCJhdWQiOiIxOTg0OTkz"))
            {                
                try
                {
                    SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    SqlCommand cmd = new SqlCommand("usp_RegisteruserConsumer", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@vendortype", request.Vendortype);
                    cmd.Parameters.AddWithValue("@username", request.Email);
                    cmd.Parameters.AddWithValue("@NormalizedUserName", request.Email);
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
                    con.Close();
                    return Ok(new  { Message = "User Register consumer account successfully...", Response = request });
                }
                catch (Exception ex)
                {

                    return BadRequest(new RegisterResponse { Message = "User Not Register ..." });

                }             
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest(Response.WriteAsync("Unauthorized: Invalid token."));
            }   
        }


        [HttpPost]
        [Route("RegisterPanelBusiness")]
        public IActionResult RegisterPanelBusiness([FromBody] RegisterRequestBusiness request)
        {
            bool emailconfirmed = true, phonenumberconfirmed = true, twofactorenable = false, lockoutenable = true, isregistrationcompleted = true;

            // Hash the password
            var hasher = new PasswordHasher<RegisterRequestBusiness>();
            request.Password = hasher.HashPassword(request, request.Password);

            var userId = Guid.NewGuid();
            var securityStamp = GenerateSecurityStamp();
            var concurrencyStamp = Guid.NewGuid().ToString();

            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("c4NjY4NTMyMTMiLCJhdWQiOiIxOTg0OTgH"))
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("usp_RegisteruserBusiness", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.Parameters.AddWithValue("@vendortype", request.Vendortype);
                cmd.Parameters.AddWithValue("@email", request.Email);
                cmd.Parameters.AddWithValue("@username", request.Email);
                cmd.Parameters.AddWithValue("@NormalizedUserName", request.Email);
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
                return Ok(new { Message = "User Register business account successfully...", Response = request });                    
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
