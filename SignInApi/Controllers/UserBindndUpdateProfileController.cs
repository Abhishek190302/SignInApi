using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignInApi.Models;

namespace SignInApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBindndUpdateProfileController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserProfileService _userProfileService;
        private readonly ISharedService _sharedService;
        public UserBindndUpdateProfileController(IUserProfileService userProfileService, ISharedService sharedService,UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userProfileService = userProfileService;
            _sharedService = sharedService;
            _httpContextAccessor = httpContextAccessor;
             _userService = userService;
        }

        [HttpPost]
        [Route("GetProfileInfo")]
        public async Task<IActionResult> GetProfileInfo(UserprofileUpdateVM profileInfomation)
        {
            try
            {
                var applicationUser = await _userService.GetUserByUserName("web@jeb.com");
                if (applicationUser != null)
                {
                    
                   string currentUserGuid = applicationUser.Id.ToString();
                    var profileInfo = await _userProfileService.GetProfileInfo(currentUserGuid);
                    profileInfo.IsVendor = applicationUser.IsVendor;

                    if (profileInfo.UserProfile == null)
                        return Redirect("/MyAccount/UserProfile");

                    profileInfo.Qualifications = await _sharedService.GetQualifications();
                    profileInfo.Countries = await _sharedService.GetCountries();
                    profileInfo.States = await _sharedService.GetStatesByCountryId(profileInfo.UserProfile.CountryID);
                    profileInfo.Cities = await _sharedService.GetCitiesByStateId(profileInfo.UserProfile.StateID);
                    profileInfo.Localities = await _sharedService.GetLocalitiesByCityId(profileInfo.UserProfile.CityID);
                    profileInfo.Pincodes = await _sharedService.GetPincodesByLocalityId(profileInfo.UserProfile.AssemblyID);
                    profileInfo.Areas = await _sharedService.GetAreasByPincodeId(profileInfo.UserProfile.PincodeID);


                    //return Ok(profileInfo);



                    if (!isValidFields(profileInfomation))
                        return BadRequest("All fields are compulsory.");

                    if (profileInfo == null)
                        return NotFound("User profile does not exist.");

                    try
                    {
                        profileInfomation.IsProfileCompleted = true;
                        await _userProfileService.UpdateUserProfile(profileInfomation);
                        return Ok("Your profile updated successfully.");
                    }
                    catch (Exception exc)
                    {
                        return StatusCode(500, exc.Message);
                    }
                }
                return NotFound("User not found");
            }
            catch (Exception exc)
            {
                return StatusCode(500, exc.Message);
            }
        }

        private bool isValidFields(UserprofileUpdateVM profileInfo)
        {
            var userProfile = profileInfo;
            if (userProfile.DateOfBirth == null || string.IsNullOrEmpty(userProfile.MaritalStatus) ||
                userProfile.QualificationId <= 0 || userProfile.CountryID <= 0 ||
                userProfile.StateID <= 0 || userProfile.CityID <= 0 || userProfile.AssemblyID <= 0 ||
                userProfile.PincodeID <= 0 || userProfile.LocalityID <= 0 || string.IsNullOrEmpty(userProfile.Address))
            {
                return false;
            }

            return true;
        }
    }
}
