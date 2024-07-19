namespace SignInApi.Models
{
    public interface IUserProfileService
    {
        Task<ProfileInfo> GetProfileInfo(string userId);
        Task UpdateUserProfile(UserprofileUpdateVM userProfile);
        Task<ApplicationUser> GetUserByUserName(string userName);
    }
}