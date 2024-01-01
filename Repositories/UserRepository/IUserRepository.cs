using rpg.Models;

namespace rpg.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> FindUserById(int id);
    }
}
