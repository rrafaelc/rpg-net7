using rpg.Models;

namespace rpg.Repositories.WeaponRepository
{
    public interface IWeaponRepository
    {
        Task<Weapon> AddWeapon(Weapon weapon);
        Task<Weapon?> FindWeaponByCharacterId(int characterId);
        Task DeleteWeapon(Weapon weapon);
    }
}
