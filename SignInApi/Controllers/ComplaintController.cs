using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        private readonly string _connectionAudit;
        public ComplaintController(IConfiguration configuration, UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("MimUser");
            _connectionAudit = configuration.GetConnectionString("AuditTrail");
        }

        [HttpPost]
        [Route("AddComplaint")]
        public async Task<IActionResult> AddComplaint([FromForm] ComplaintRequest request)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description) || request.File == null)
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
                            var userProfile = await GetProfileByOwnerGuid(currentUserGuid);


                            if (request.File == null || request.File.Length == 0)
                                return BadRequest("No file uploaded.");

                            var imagePath = Path.Combine("wwwroot/images/logos", request.File.FileName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await request.File.CopyToAsync(stream);
                            }

                            var imageUrl = $"/images/logos/" + currentUserGuid + "/" + request.File.FileName + "";

                            var complaint = new Complaint
                            {
                                OwnerGuid = currentUserGuid,
                                Mobile = applicationUser.PhoneNumber,
                                Email = applicationUser.Email,
                                Name = userProfile?.Name,
                                Title = request.Title,
                                Description = request.Description,
                                Date = DateTime.Now,
                                ImagePath = imageUrl
                            };

                            await AddAsync(complaint);

                            return Ok(new { Message = "Your complaint submitted successfully!", Complaints = complaint });

                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { StatusCode = 500, Message = "An error occurred while saving the complaint.", Details = ex.Message });
                        }
                    }
                    return NotFound(new { StatusCode = 500, Message = "User not found." });
                }
                return Unauthorized(new { StatusCode = 500, Message = "User is not authenticated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        private async Task<UserProfile> GetProfileByOwnerGuid(string ownerGuid)
        {
            UserProfile profile = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OwnerGuid, Name FROM [dbo].[UserProfile] WHERE OwnerGuid = @OwnerGuid";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OwnerGuid", ownerGuid);

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        await connection.OpenAsync();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            profile = new UserProfile
                            {
                                OwnerGuid = row["OwnerGuid"].ToString(),
                                Name = row["Name"].ToString()
                            };
                        }
                    }
                }
            }

            return profile;
        }


        private async Task AddAsync(Complaint complaint)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionAudit))
                {
                    string query = @"INSERT INTO [dbo].[Complaints] (OwnerGuid,Date,Name,Email,Mobile,Title,Description,ImagePath)
                             VALUES (@OwnerGuid,@Date, @Name,@Email, @Mobile,@Title, @Description, @ImagePath)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OwnerGuid", complaint.OwnerGuid);
                        command.Parameters.AddWithValue("@Date", complaint.Date);
                        command.Parameters.AddWithValue("@Name", complaint.Name);
                        command.Parameters.AddWithValue("@Email", complaint.Email);
                        command.Parameters.AddWithValue("@Mobile", complaint.Mobile);
                        command.Parameters.AddWithValue("@Title", complaint.Title);
                        command.Parameters.AddWithValue("@Description", complaint.Description);
                        command.Parameters.AddWithValue("@ImagePath", complaint.ImagePath);

                        connection.Open();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch(Exception ex) 
            {
                throw;
            }
            
        }
    }
}
