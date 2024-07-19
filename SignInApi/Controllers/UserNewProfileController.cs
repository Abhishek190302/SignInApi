using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;
using System.Reflection;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNewProfileController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserNewProfileService _userNewProfileService;
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
            var applicationUser = await _userService.GetUserByUserName("web@teb.com");
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
                            TimeZoneOfCountry = userProfileVM.TimeZoneOfCountry,
                        };

                        var imagePath = Path.Combine("wwwroot/images/logos/", userProfileVM.File.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await userProfileVM.File.CopyToAsync(stream);
                        }
                        var imageUrl = $"/images/logos/" + currentUserGuid + "/" + userProfileVM.File.FileName + "";

                        await _userNewProfileService.AddUserProfile(userProfile, imageUrl);
                        return Ok("Your profile created successfully.");
                    }
                    else
                    {
                        // Update existing profile
                        userProfile.FirstName = userProfileVM.FirstName;
                        userProfile.LastName = userProfileVM.LastName;
                        userProfile.Gender = userProfileVM.Gender;
                        userProfile.UpdatedDate = DateTime.UtcNow;

                        await _userNewProfileService.UpdateUserProfile(userProfile);
                        return Ok("Your profile updated successfully.");
                    }

                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            return NotFound("User not found");
        }
    }
}
