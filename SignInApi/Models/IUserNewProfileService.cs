﻿namespace SignInApi.Models
{
    public interface IUserNewProfileService
    {
        Task<UserNewProfile> GetProfileByOwnerGuid(string ownerGuid);
        Task AddUserProfile(UserNewProfile userProfile, string imageURL);
        Task UpdateUserProfile(UserNewProfile userProfile,string imageURL);
        Task<string> UploadProfileImage(IFormFile file, string ownerGuid);
    }
}