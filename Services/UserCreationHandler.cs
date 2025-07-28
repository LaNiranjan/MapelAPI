using MapelRestAPI.Entities;
using MapelRestAPI.Interfaces;

namespace MapelRestAPI.Services
{
    public class UserCreationHandler
    {
        private readonly IUserService _userService;

        public UserCreationHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<string>> HandleAsync(List<AppUser> users)
        {
            return await _userService.CreateUsersAsync(users);
        }

    }
}
