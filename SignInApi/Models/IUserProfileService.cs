namespace SignInApi.Models
{
    public interface IUserProfileService
    {
        Task<ProfileInfo> GetProfileInfo(string userId);
        Task UpdateUserProfile(UserprofileUpdateVM userProfile, string ownerguid);
        Task<ApplicationUser> GetUserByUserName(string userName);
    }
}