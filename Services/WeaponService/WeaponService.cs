using System.Security.Claims;
using AutoMapper;
using rpg.Dtos.Character;
using rpg.Dtos.Weapon;
using rpg.Models;
using rpg.Repositories.CharacterRepository;
using rpg.Repositories.WeaponRepository;

namespace rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly IWeaponRepository _weaponRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public WeaponService(IWeaponRepository weaponRepository, ICharacterRepository characterRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _weaponRepository = weaponRepository;
            _characterRepository = characterRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<CharacterResponseDto>> AddWeapon(AddWeaponRequestDto newWeapon)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();

            try
            {
                var character = await _characterRepository.FindCharacterByIdAndUserId(newWeapon.CharacterId, GetUserId())
                ?? throw new Exception("Character not found");

                var weaponExists = await _weaponRepository.FindWeaponByCharacterId(character.Id);
                if (weaponExists is not null)
                {
                    await _weaponRepository.DeleteWeapon(weaponExists);
                }

                var weapon = _mapper.Map<Weapon>(newWeapon);
                weapon.Character = character;
                await _weaponRepository.AddWeapon(weapon);
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
