using MapelRestAPI.Entities;

namespace MapelRestAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> CreateUsersAsync(List<AppUser> users);
        Task<string> InviteExternalUserAsync(string email);
    }
}
