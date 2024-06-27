using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignInApi.Models;
using System.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using System.Text;
using Twilio.Http;
using Microsoft.AspNetCore.Cors;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("c4NjweNTMyMTMiLCfthWQiOiIxOjhtOT3r"))
            {
                try
                {
                    SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    SqlCommand cmd = new SqlCommand("usp_checkuserlogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@mobile", login.Mobile);
                    cmd.Parameters.AddWithValue("@password", login.Password);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new LoginResponse { Message = "User login successfully..." });
                    }
                    else
                    {
                        return BadRequest(new LoginResponse { Message = "User not login..." });
                    }
                }
                catch (Exception ex)
                {

                    return BadRequest(new LoginResponse { Message = "User Not Login ..." });

                }
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest(Response.WriteAsync("Unauthorized: Invalid token."));
            }
        }
    }
}
