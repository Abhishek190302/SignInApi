using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using Twilio.Http;
using Twilio.Jwt.AccessToken;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("generate-token")]
        public IActionResult GenerateToken()
        {
            var token = GenerateRandomToken();
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("usp_Tokendetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@token", token);
            cmd.ExecuteNonQuery();
            con.Close();
            return Ok(new { token });
        }

        //private string GenerateRandomToken()
        //{
        //    using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
        //    {
        //        var randomBytes = new byte[18];
        //        rngCryptoServiceProvider.GetBytes(randomBytes);
        //        return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        //    }
        //}


        public static string GenerateRandomToken()
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
