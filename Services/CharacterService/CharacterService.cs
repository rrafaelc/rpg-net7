using System.Security.Claims;
using AutoMapper;
using rpg.Dtos.Character;
using rpg.Models;
using rpg.Repositories.CharacterRepository;
using rpg.Repositories.SkillRepository;
using rpg.Repositories.UserRepository;

namespace rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly ICharacterRepository _characterRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ISkillRepository _skillRepository;

        public CharacterService(IMapper mapper, ICharacterRepository characterRepository, IUserRepository userRepository, ISkillRepository skillRepository, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _characterRepository = characterRepository;
            _userRepository = userRepository;
            _skillRepository = skillRepository;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<CharacterResponseDto>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();

            try
            {
                var character = _mapper.Map<Character>(newCharacter);
                character.User = await _userRepository.FindUserById(GetUserId());

                var characterCreated = await _characterRepository.AddCharacter(character);
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(characterCreated);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<CharacterResponseDto>>> GetAllCharacters()
        {
            var dbCharacters = await _characterRepository.FindCharacters(GetUserId());
            var serviceResponse = new ServiceResponse<List<CharacterResponseDto>>
            {
                Data = dbCharacters.Select(x => _mapper.Map<CharacterResponseDto>(x)).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();
            var dbCharacter = await _characterRepository.FindCharacterById(id, GetUserId());

            if (dbCharacter is not null)
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(dbCharacter);
            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character not found";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();
            try
            {
                if (updatedCharacter == null)
                    throw new Exception("Invalid input: updatedCharacter is null");

                var character = await _characterRepository.FindCharacterById(updatedCharacter.Id, GetUserId())
                ?? throw new Exception($"Character with id '{updatedCharacter.Id}' not found");
                character.Name = updatedCharacter.Name ?? character.Name;
                character.HitPoints = updatedCharacter.HitPoints ?? character.HitPoints;
                character.Strength = updatedCharacter.Strength ?? character.Strength;
                character.Defense = updatedCharacter.Defense ?? character.Defense;
                character.Intelligence = updatedCharacter.Intelligence ?? character.Intelligence;
                character.Class = updatedCharacter.Class ?? character.Class;
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(character);

                await _characterRepository.UpdateCharacter(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task DeleteCharacter(int id)
        {
            var dbCharacter = await _characterRepository.FindCharacterById(id, GetUserId())
            ?? throw new Exception($"Character with id '{id}' not found");
            await _characterRepository.DeleteCharacter(dbCharacter);
        }

        public async Task<ServiceResponse<CharacterResponseDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();

            try
            {
                var character = await _characterRepository.FindCharacterById(newCharacterSkill.CharacterId, GetUserId())
                ?? throw new Exception($"Character with id '{newCharacterSkill.CharacterId}' not found");
                var skill = await _skillRepository.FindSkillById(newCharacterSkill.SkillId)
                ?? throw new Exception($"Skill with id '{newCharacterSkill.SkillId}' not found");
                character.Skills!.Add(skill);
                await _characterRepository.UpdateCharacter(character);
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
