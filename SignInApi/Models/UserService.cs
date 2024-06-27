using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace SignInApi.Models
{
    public class UserService
    {
        
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            return await _userRepository.GetUserByUserName(userName);
        }
    }
}
