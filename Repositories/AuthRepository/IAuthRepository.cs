using rpg.Models;

namespace rpg.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Task<User> AddUser(User user);
        Task<User?> FindUserByUsername(string username);
        Task<bool> UserExists(string username);
    }
}
