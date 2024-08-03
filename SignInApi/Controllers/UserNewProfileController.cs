using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Reflection;
using Twilio.TwiML.Messaging;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNewProfileController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserNewProfileService _userNewProfileService;
        public string TimeZoneOfCountry { get; set; } = "India Standard Time";
        public UserNewProfileController(IUserNewProfileService userNewProfileService, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _userNewProfileService = userNewProfileService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        [HttpPost]
        [Route("CreateOrUpdateProfile")]
        public async Task<IActionResult> CreateOrUpdateProfile([FromForm] UserNewProfileVM userProfileVM)
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
                        var userProfile = await _userNewProfileService.GetProfileByOwnerGuid(currentUserGuid);
                        if (userProfile == null)
                        {
                            // Create new profile
                            userProfile = new UserNewProfile
                            {
                                OwnerGuid = currentUserGuid,
                                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                FirstName = userProfileVM.FirstName,
                                LastName = userProfileVM.LastName,
                                Gender = userProfileVM.Gender,
                                CreatedDate = DateTime.UtcNow,
                                TimeZoneOfCountry = TimeZoneOfCountry,
                            };


                            if (userProfileVM.File == null || userProfileVM.File.Length == 0)
                                return BadRequest("No file uploaded.");

                            var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                            if (!Directory.Exists(userDirectory))
                            {
                                Directory.CreateDirectory(userDirectory);
                            }

                            var imagePath = Path.Combine(userDirectory, userProfileVM.File.FileName);

                            //var imagePath = Path.Combine("wwwroot/images/logos/", userProfileVM.File.FileName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await userProfileVM.File.CopyToAsync(stream);
                            }
                            var imageUrl = $"/images/logos/{currentUserGuid}/{userProfileVM.File.FileName}";

                            //var imageUrl = $"/images/logos/" + userProfileVM.File.FileName + "";

                            await _userNewProfileService.AddUserProfile(userProfile, imageUrl);
                            return Ok(new { Message = "Your profile created successfully.", Userprofile = userProfile });
                        }
                        else
                        {
                            if (userProfileVM.File == null || userProfileVM.File.Length == 0)
                                return BadRequest("No file uploaded.");

                            var userDirectory = Path.Combine("wwwroot/images/logos", currentUserGuid);
                            if (!Directory.Exists(userDirectory))
                            {
                                Directory.CreateDirectory(userDirectory);
                            }

                            var imagePath   = Path.Combine(userDirectory, userProfileVM.File.FileName);

                            //var imagePath = Path.Combine("wwwroot/images/logos/", userProfileVM.File.FileName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await userProfileVM.File.CopyToAsync(stream);
                            }

                            var imageUrl = $"/images/logos/{currentUserGuid}/{userProfileVM.File.FileName}";
                            //var imageUrl = $"/images/logos/" + userProfileVM.File.FileName + "";

                            // Update existing profile
                            userProfile.FirstName = userProfileVM.FirstName;
                            userProfile.LastName = userProfileVM.LastName;
                            userProfile.Gender = userProfileVM.Gender;
                            //userProfile.UpdatedDate = DateTime.UtcNow;

                            await _userNewProfileService.UpdateUserProfile(userProfile, imageUrl);
                            return Ok(new { Message = "Your profile Updated successfully.", Userprofile = userProfile });
                        }

                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }
                return NotFound("User not found");
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("GetMobileEmail")]
        public async Task<IActionResult> GetMobileEmail()
        {
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
                            var mobile = applicationUser.PhoneNumber;
                            var email = applicationUser.Email;
                            return Ok(new { mobile, email });
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
