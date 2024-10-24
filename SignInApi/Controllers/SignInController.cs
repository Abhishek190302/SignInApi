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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly TokenService _tokenService;
        public SignInController(IConfiguration configuration, TokenService tokenService) 
        {
            _configuration = configuration;
            _tokenService = tokenService;

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
        [Route("ClaimForgotPassword")]
        public async Task<IActionResult> ClaimForgotPassword([FromBody] CalimForgotpassword request)
        {
            string otp = GenerateOtp();
            string message = $"{otp} is your One-Time Password, valid for 10 minutes only. Please do not share your OTP with anyone.";

            var input = request.MobileorEmail;
            if (IsValidMobile(input))
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser")))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    var mobile = request.MobileorEmail;
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
                            using (SqlCommand cmd1 = new SqlCommand("update AspNetUsers set Otp=@otp,otpsenddate=GETDATE(),Response=@response where PhoneNumber=@mobile", con1))
                            {
                                cmd1.Parameters.AddWithValue("@mobile", mobile);
                                cmd1.Parameters.AddWithValue("@otp", otp);
                                cmd1.Parameters.AddWithValue("@response", message);
                                cmd1.ExecuteNonQuery();
                            }
                        }
                        return Ok(new { Message = "OTP is sent to your registered " + mobile + " mobile number.", Otp = otp });
                        //return Ok(new { Message = "OTP is sent to your registered mobile number.", OTP = otp });
                    }
                    else
                    {
                        return BadRequest(new ForgotpasswordResponse { Message = "OTP not sent to registered mobile number" });
                    }
                }
            }
            else if(IsValidEmail(input))
            {
                string OTP = GenerateOtp();
                string Message = $"{otp} is your One-Time Password, valid for 10 minutes only. Please do not share your OTP with anyone.";
                string Otp = "";
                string result = "";


                result = await SendOtpToEmail(request.MobileorEmail, otp);

                using (SqlConnection con1 = new SqlConnection(_configuration.GetConnectionString("MimUser")))
                {
                    if (con1.State == ConnectionState.Closed) { con1.Open(); }
                    using (SqlCommand cmd1 = new SqlCommand("update AspNetUsers set Otp=@otp,otpsenddate=GETDATE(),Response=@response where Email=@email", con1))
                    {
                        cmd1.Parameters.AddWithValue("@email", request.MobileorEmail);
                        cmd1.Parameters.AddWithValue("@otp", otp);
                        cmd1.Parameters.AddWithValue("@response", message);
                        cmd1.ExecuteNonQuery();
                    }
                }
                return Ok(new { Message = "OTP is sent to your registered " + request.MobileorEmail + " email id.", Otp = otp });
                //return Ok(new { Message = "OTP is sent to your Emailid.", OTP = otp });
            }

            return Ok();
        }

        private bool IsValidMobile(string input)
        {
            // Use regex to validate if the input is a valid mobile number
            return Regex.IsMatch(input, @"^[6-9]\d{9}$");
        }

        private bool IsValidEmail(string input)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
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


        [HttpPost]
        [Route("CalimRegisteration")]
        public IActionResult CalimRegisteration([FromBody] CalimRegisterationVM request)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
            ErrorResponse errorResponse = new ErrorResponse();
            var hasher = new PasswordHasher<CalimRegisterationVM>();
            request.Password = hasher.HashPassword(request, request.Password);

            if (con.State == ConnectionState.Closed) { con.Open(); }
            SqlCommand cmd = new SqlCommand("select Id,Email,PhoneNumber from AspNetUsers where (PhoneNumber = @MobileOrEmail OR Email = @MobileOrEmail);", con);
            cmd.Parameters.AddWithValue("@MobileOrEmail", request.MobileorEmail);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            con.Close();
            if (dt1.Rows.Count > 0)
            {
                var Id = dt1.Rows[0]["Id"].ToString();
                var Email = dt1.Rows[0]["Email"].ToString();
                var PhoneNumber = dt1.Rows[0]["PhoneNumber"].ToString();

                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd1 = new SqlCommand("usp_updateForgotpassword", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@id", Id);
                cmd1.Parameters.AddWithValue("@password", request.Password);
                cmd1.Parameters.AddWithValue("@confirmpassword", request.Confirmpassword);
                cmd1.ExecuteNonQuery();
                con.Close();


                SqlCommand cmdbussi = new SqlCommand("SELECT * FROM [dbo].[AspNetUsers]  WHERE (PhoneNumber = @MobileOrEmail OR Email = @MobileOrEmail) AND IsRegistrationCompleted = 1", con);
                cmdbussi.Parameters.AddWithValue("@MobileOrEmail", request.MobileorEmail);
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
                return Ok(new { Message = "Password Update Sucessfully...", Response = request, Token = token });                
            }
            else
            {
                return BadRequest(new ForgotpasswordResponse { Message = "Otp Does Not Match" });
            }
            
        }



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




        [HttpPost]
        [Route("VerifyOtpClaimFogotpassword")]
        public IActionResult VerifyOtpClaimFogotpassword([FromBody] VerifyforgotpasswordRequest request)
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
                var fromAddress = new MailAddress("contact@myinteriormart.com", "MyinteriorMart");
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


        [HttpPost]
        [Route("ClaimVerifyOtp")]
        public IActionResult ClaimVerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("kpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ"))
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("select Otp from [dbo].[AspNetUsers] where Otp=@otp;", con);
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
                        SqlCommand cmd1 = new SqlCommand("select Otp,PhoneNumber from [dbo].[AspNetUsers] where Otp=@otp and PhoneNumber=@mobile;", con);
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
        [Route("ClaimVerifyOtpEmail")]
        public IActionResult ClaimVerifyOtpEmail([FromBody] VerifyOtpRequestEmail request)
        {
            var Token = Request.Headers["Authorization"].ToString();

            if (Token.Contains("SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV"))
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MimUser"));
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("select Otp from [dbo].[AspNetUsers] where Otp=@otp;", con);
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
                        SqlCommand cmd1 = new SqlCommand("select Otp,Email from [dbo].[AspNetUsers] where Otp=@otp and Email=@email;", con);
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
