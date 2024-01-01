using rpg.Models;

namespace rpg.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<ServiceResponse<int>> Register(User user, string password);
    }
}
