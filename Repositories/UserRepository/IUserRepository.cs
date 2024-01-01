using rpg.Models;

namespace rpg.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> FindUserById(int id);
        Task<User> AddUser(User user);
        Task<User?> FindUserByUsername(string username);
        Task<bool> UserExists(string username);
    }
}
