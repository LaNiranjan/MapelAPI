using MapelRestAPI.Interfaces;

namespace MapelRestAPI.Services
{
    public class UserInvitaionHandler
    {
        private readonly IUserService _userService;

        public UserInvitaionHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<string> HandleAsync(string email)
        {
            return await _userService.InviteExternalUserAsync(email);
        }
    }
}
