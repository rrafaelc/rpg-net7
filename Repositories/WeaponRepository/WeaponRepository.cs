using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Models;

namespace rpg.Repositories.WeaponRepository
{
    public class WeaponRepository : IWeaponRepository
    {
        private readonly DataContext _context;
        public WeaponRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Weapon> AddWeapon(Weapon weapon)
        {
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
            return weapon;
        }

        public async Task<Weapon?> FindWeaponByCharacterId(int characterId)
        {
            return await _context.Weapons.FirstOrDefaultAsync(x => x.CharacterId == characterId);
        }

        public async Task DeleteWeapon(Weapon weapon)
        {
            _context.Weapons.Remove(weapon);
            await _context.SaveChangesAsync();
        }
    }
}
