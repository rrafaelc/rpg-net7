using rpg.Dtos.Character;
using rpg.Dtos.Weapon;
using rpg.Models;

namespace rpg.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<CharacterResponseDto>> AddWeapon(AddWeaponRequestDto newWeapon);
    }
}
