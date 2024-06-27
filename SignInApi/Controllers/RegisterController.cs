using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using static System.Net.WebRequestMethods;
using System.Data.SqlClient;
using System.Data;
using Twilio.TwiML.Messaging;
using Microsoft.AspNetCore.Cors;
using System.Security.Cryptography;

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
            var id = GenerateRandomid();
            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("c4NjY4NTMyMTMiLCJhdWQiOiIxOTg0OTkz"))
            {                
                try
                {
                    SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    SqlCommand cmd = new SqlCommand("usp_RegisteruserConsumer", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@vendortype", request.Vendortype);
                    cmd.Parameters.AddWithValue("@email", request.Email);
                    cmd.Parameters.AddWithValue("@mobile", request.Mobile);
                    cmd.Parameters.AddWithValue("@password", request.Password);
                    cmd.Parameters.AddWithValue("@confirmpassword", request.Confirmpassword);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return Ok(new RegisterResponse { Message = "User Register consumer account successfully..." });
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
            var id = GenerateRandomid();
            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("c4NjY4NTMyMTMiLCJhdWQiOiIxOTg0OTgH"))
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("usp_RegisteruserBusiness", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@vendortype", request.Vendortype);
                cmd.Parameters.AddWithValue("@email", request.Email);
                cmd.Parameters.AddWithValue("@mobile", request.Mobile);
                cmd.Parameters.AddWithValue("@password", request.Password);
                cmd.Parameters.AddWithValue("@confirmpassword", request.Confirmpassword);
                cmd.Parameters.AddWithValue("@businesscategory", request.Businesscategory);
                cmd.ExecuteNonQuery();
                con.Close();
                return Ok(new RegisterResponse { Message = "User Register business account successfully..." });                    
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
    }
}
