using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SignInApi.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Twilio.Http;
using Twilio.Jwt.AccessToken;
using static System.Net.WebRequestMethods;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class SignInController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SignInController(IConfiguration configuration) 
        {
            _configuration = configuration;
            
        }

        //[HttpPost]
        //[Route("SendOtp")]
        //public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        //{

        //    var Token = Request.Headers["Authorization"].ToString();

        //    if (Token.Contains("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"))
        //    {
        //        string otp = GenerateOtp();
        //        string message = $"{otp} is your One-Time Password, valid for 10 minutes only. Please do not share your OTP with anyone.";
        //        string Otp = "";
        //        string result = "";

        //        if (request.CountryCode == "+91")
        //        {
        //            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
        //            if (con.State == ConnectionState.Closed) { con.Open(); }
        //            SqlCommand cmd = new SqlCommand("usp_signOtpformobile", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@contrycode", request.CountryCode);
        //            cmd.Parameters.AddWithValue("@mobile", request.Mobile);
        //            cmd.Parameters.AddWithValue("@otp", otp);
        //            cmd.Parameters.AddWithValue("@response", message);
        //            cmd.ExecuteNonQuery();
        //            con.Close();

        //            return Ok(new { Message = "OTP is sent to your registered " + request.Mobile + " mobile number.", Otp = otp });
        //        }
        //        else
        //        {
        //            return Ok(new { Message = "OTP not sent to your registered mobile number."});
        //        }
        //    }
        //    else
        //    {
        //        // Token does not contain the specified substring, return 401 Unauthorized
        //        Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        return BadRequest( Response.WriteAsync("Unauthorized: Invalid token."));
        //    }    
        //}


        [HttpPost]
        [Route("SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            var token = Request.Headers["Authorization"].ToString();

            if (token.Contains("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"))
            {
                string otp = GenerateOtp();
                string message = $"{otp} is your One-Time Password, valid for 10 minutes only. Please do not share your OTP with anyone.";
                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    string apiUrl = $"vas.hexaroute.com/api.php?username=myinteriormart&password=pass1234&route=1&sender=MyIntM&mobile[]={request.Mobile}&message[]={otp} is your login code and is valid for 10 minutes. Do not share the OTP with anyone. my Interior Mart Team&te_id=1207172509456648630";
                    HttpResponseMessage response = await client.GetAsync("http://" + apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Failed to send OTP via SMS.");
                    }
                }

                if (request.CountryCode == "+91")
                {
                    using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser")))
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        using (SqlCommand cmd = new SqlCommand("usp_signOtpformobile", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@contrycode", request.CountryCode);
                            cmd.Parameters.AddWithValue("@mobile", request.Mobile);
                            cmd.Parameters.AddWithValue("@otp", otp);
                            cmd.Parameters.AddWithValue("@response", message);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }

                    return Ok(new { Message = "OTP is sent to your registered " + request.Mobile + " mobile number.", Otp = otp });
                }
                else
                {
                    return Ok(new { Message = "OTP not sent to your registered mobile number." });
                }
            }
            else
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest("Unauthorized: Invalid token.");
            }
        }



        [HttpPost]
        [Route("SendOtpEmail")]
        public async Task<IActionResult> SendOtpEmail([FromBody] OtpRequestEmail request)
        {

            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCe3"))
            {
                string otp = GenerateOtp();
                string message = $"{otp} is your One-Time Password, valid for 10 minutes only. Please do not share your OTP with anyone.";
                string Otp = "";
                string result = "";

               
                result = await SendOtpToEmail(request.Email, otp);
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("usp_signOtpforemail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@contrycode", request.CountryCode);
                cmd.Parameters.AddWithValue("@email", request.Email);
                cmd.Parameters.AddWithValue("@otp", otp);
                cmd.Parameters.AddWithValue("@response", message);
                cmd.ExecuteNonQuery();
                con.Close();

                return Ok(new { Message = "OTP is sent to your registered " + request.Email + " email id.", Otp = otp });
               
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest(Response.WriteAsync("Unauthorized: Invalid token."));
            }
        }




        //[HttpPost]
        //[Route("ForgotPassword")]
        //public IActionResult ForgotPassword([FromBody] ForgotpasswordRequest request)
        //{
        //    var Token = Request.Headers["Authorization"].ToString();

        //    if (Token.Contains("df2359eb-79c7-45b9-9c02-2c66e289d5c8"))
        //    {
        //        string otp = GenerateOtp();
        //        string message = $"{otp} is your One-Time Password, valid for 10 minutes only. Please do not share your OTP with anyone.";
        //        string OTP = "";

        //        SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
        //        if (con.State == ConnectionState.Closed) { con.Open(); }
        //        SqlCommand cmd = new SqlCommand("usp_Verifymobile", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@mobile", request.Mobile);
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        con.Close();
        //        if (dt.Rows.Count > 0)
        //        {
        //            var mobile = dt.Rows[0]["PhoneNumber"].ToString();
        //            if (mobile != null)
        //            {
        //                SqlConnection con1 = new SqlConnection(_configuration.GetConnectionString("MimUser"));
        //                if (con1.State == ConnectionState.Closed) { con1.Open(); }
        //                SqlCommand cmd1 = new SqlCommand("usp_storeOtpdetails", con1);
        //                cmd1.CommandType = CommandType.StoredProcedure;
        //                cmd1.Parameters.AddWithValue("@mobile", mobile);
        //                cmd1.Parameters.AddWithValue("@otp", otp);
        //                cmd1.Parameters.AddWithValue("@response", message);
        //                cmd1.ExecuteNonQuery();
        //                con1.Close();
        //                return Ok(new { Message = "OTP is sent to your registered mobile number.", OTP = otp });
        //            }
        //            else
        //            {
        //                return BadRequest(new ForgotpasswordResponse { Message = "OTP not send registered mobile number" });
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest(new ForgotpasswordResponse { Message = "Mobile number does not match register mobile number" });
        //        }
        //    }
        //    else
        //    {
        //        // Token does not contain the specified substring, return 401 Unauthorized
        //        Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        return BadRequest( Response.WriteAsync("Unauthorized: Invalid token."));
        //    }      
        //}

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotpasswordRequest request)
        {
            var token = Request.Headers["Authorization"].ToString();

            if (token.Contains("df2359eb-79c7-45b9-9c02-2c66e289d5c8"))
            {
                string otp = GenerateOtp();
                string message = $"{otp} is your One-Time Password, valid for 10 minutes only. Please do not share your OTP with anyone.";

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser")))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    using (SqlCommand cmd = new SqlCommand("usp_Verifymobile", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@mobile", request.Mobile);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                var mobile = dt.Rows[0]["PhoneNumber"].ToString();
                                if (!string.IsNullOrEmpty(mobile))
                                {
                                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                                    {
                                        string apiUrl = $"vas.hexaroute.com/api.php?username=myinteriormart&password=pass1234&route=1&sender=MyIntM&mobile[]={mobile}&message[]={otp} is your login code and is valid for 10 minutes. Do not share the OTP with anyone. my Interior Mart Team&te_id=1207172509456648630";
                                        HttpResponseMessage response = await client.GetAsync("http://" + apiUrl);
                                        if (!response.IsSuccessStatusCode)
                                        {
                                            return StatusCode((int)response.StatusCode, "Failed to send OTP via SMS.");
                                        }
                                    }

                                    using (SqlConnection con1 = new SqlConnection(_configuration.GetConnectionString("MimUser")))
                                    {
                                        if (con1.State == ConnectionState.Closed) { con1.Open(); }
                                        using (SqlCommand cmd1 = new SqlCommand("usp_storeOtpdetails", con1))
                                        {
                                            cmd1.CommandType = CommandType.StoredProcedure;
                                            cmd1.Parameters.AddWithValue("@mobile", mobile);
                                            cmd1.Parameters.AddWithValue("@otp", otp);
                                            cmd1.Parameters.AddWithValue("@response", message);
                                            cmd1.ExecuteNonQuery();
                                        }
                                    }
                                    return Ok(new { Message = "OTP is sent to your registered mobile number.", OTP = otp });
                                }
                                else
                                {
                                    return BadRequest(new ForgotpasswordResponse { Message = "OTP not sent to registered mobile number" });
                                }
                            }
                            else
                            {
                                return BadRequest(new ForgotpasswordResponse { Message = "Mobile number does not match registered mobile number" });
                            }
                        }
                    }
                }
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest("Unauthorized: Invalid token.");
            }
        }

        [HttpPost]
        [Route("VerifyOtpFogotpassword")]
        public IActionResult VerifyOtpFogotpassword([FromBody] VerifyforgotpasswordRequest request)
        {
            var hasher = new PasswordHasher<VerifyforgotpasswordRequest>();
            request.Password = hasher.HashPassword(request, request.Password);

            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("12345abcde67890fghij12345klmno67890pqr"))
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("usp_Verifyforgotpassword", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@otp", request.Otp);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    var Id = dt.Rows[0]["Id"].ToString();
                    var verify = dt.Rows[0]["Otp"].ToString();

                    if (verify != null)
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        SqlCommand cmd1 = new SqlCommand("usp_updateForgotpassword", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@id", Id);
                        cmd1.Parameters.AddWithValue("@password", request.Password);
                        cmd1.Parameters.AddWithValue("@confirmpassword", request.ConfirmPassword);
                        cmd1.ExecuteNonQuery();
                        con.Close();
                        return Ok(new { Message = "Password Update Sucessfully...", Response = request });
                    }
                    else
                    {
                        return BadRequest(new ForgotpasswordResponse { Message = "Password Does Not Update" });
                    }
                }
                else
                {
                    return BadRequest(new ForgotpasswordResponse { Message = "Otp Does Not Match" });
                }
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest(Response.WriteAsync("Unauthorized: Invalid token."));
            }
        }

        private async Task<string> SendOtpToEmail(string emailAddress, string otpCode)
        {
            try
            {
                var fromAddress = new MailAddress("contact@myinteriormart.com", "Hamza");
                var toAddress = new MailAddress(emailAddress);
                var fromPassword = "Hamza@313#";
                var subject = "Your OTP Code";
                string body = $"Verify this otp your register email id Your OTP is {otpCode}";

                using (var message = new MailMessage())
                {
                    message.From = fromAddress;
                    message.To.Add(toAddress);
                    message.Subject = subject;
                    message.Body = body;

                    using (var smtp = new SmtpClient("mail.myinteriormart.com", 587))
                    {
                        smtp.EnableSsl = false;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);

                        // Log the command to verify it's sent correctly
                        smtp.SendCompleted += (s, e) =>
                        {
                            if (e.Error != null)
                            {        
                                Console.WriteLine($"Send failed: {e.Error.Message}");
                            }
                            else
                            {
                                Console.WriteLine("Email sent successfully.");
                            }
                        };

                        await smtp.SendMailAsync(message);
                    }
                }

                return "OTP sent to your email successfully";
            }
            catch (SmtpException ex)
            {
                return $"Failed to send OTP: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [HttpPost]
        [Route("VerifyOtp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("kpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ"))
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("verifyOtp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@otp", request.Otp);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    var otp = dt.Rows[0]["Otp"].ToString();
                    if (otp != null)
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        SqlCommand cmd1 = new SqlCommand("verifyOtpwithPhonenumber", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@mobile", request.Mobile);
                        cmd1.Parameters.AddWithValue("@otp", request.Otp);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        con.Close();
                        if (dt1.Rows.Count > 0)
                        {
                            return Ok(new VerifyOtpResponse { Message = "OTP verified with register " + request.Mobile + " mobile number successfully." });
                        }
                        else
                        {
                            return BadRequest(new VerifyOtpResponse { Message = "Phone number does not match" });
                        }
                    }
                    else
                    {
                        return BadRequest(new VerifyOtpResponse { Message = "Invalid otp with register mobile number." });
                    }
                }
                else
                {
                    return BadRequest(new VerifyOtpResponse { Message = "Invalid otp with register mobile number." });
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
        [Route("VerifyOtpEmail")]
        public IActionResult VerifyOtpEmail([FromBody] VerifyOtpRequestEmail request)
        {
            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV"))
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("verifyOtp_email", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@otp", request.Otp);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    var otp = dt.Rows[0]["Otp"].ToString();
                    if (otp != null)
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        SqlCommand cmd1 = new SqlCommand("verifyOtpwithemail", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@email", request.Email);
                        cmd1.Parameters.AddWithValue("@otp", request.Otp);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        con.Close();
                        if (dt1.Rows.Count > 0)
                        {
                            return Ok(new VerifyOtpResponse { Message = "OTP verified with register " + request.Email + " email id successfully." });
                        }
                        else
                        {
                            return BadRequest(new VerifyOtpResponse { Message = "email id does not match" });
                        }
                    }
                    else
                    {
                        return BadRequest(new VerifyOtpResponse { Message = "Invalid otp with register email id." });
                    }
                }
                else
                {
                    return BadRequest(new VerifyOtpResponse { Message = "Invalid otp with register email id." });
                }
            }
            else
            {
                // Token does not contain the specified substring, return 401 Unauthorized
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return BadRequest(Response.WriteAsync("Unauthorized: Invalid token."));
            }  
        }

        protected string GenerateOtp()
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            for (int i = 0; i < 4; i++)
            {
                // It will not allow repetition of characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString()))
                {
                    strrandom += charArr.GetValue(pos);
                }
                else
                {
                    i--;
                }
            }
            return strrandom;
        }


        private string GenerateRandomToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[24];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
            }
        }
    }
}
