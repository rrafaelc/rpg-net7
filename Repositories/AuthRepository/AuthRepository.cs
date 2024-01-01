using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Models;

namespace rpg.Repositories.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> FindUserByUsername(string username) =>
            await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

        public async Task<bool> UserExists(string username) =>
            await _context.Users.AnyAsync(x => x.Username == username);
    }
}
