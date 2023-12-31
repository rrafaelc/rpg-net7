using AutoMapper;
using rpg.Dtos.Character;
using rpg.Models;

namespace rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{ Id = 1, Name = "Sam"},
        };
        private readonly IMapper _mapper;

        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();
            var character = _mapper.Map<Character>(newCharacter);
            character.Id = characters.Max(x => x.Id) + 1;
            characters.Add(character);
            serviceResponse.Data = _mapper.Map<CharacterResponseDto>(character);
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<CharacterResponseDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<CharacterResponseDto>>
            {
                Data = characters.Select(x => _mapper.Map<CharacterResponseDto>(x)).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> GetCharacterById(int id)
        {
            var character = characters.FirstOrDefault(x => x.Id == id);

            var serviceResponse = new ServiceResponse<CharacterResponseDto>();
            serviceResponse.Data = _mapper.Map<CharacterResponseDto>(character);
            return serviceResponse;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();
            try
            {
                if (updatedCharacter == null)
                    throw new Exception("Invalid input: updatedCharacter is null");

                var character = characters.FirstOrDefault(x => x.Id == updatedCharacter.Id)
                ?? throw new Exception($"Character with id '{updatedCharacter.Id}' not found");
                character.Name = updatedCharacter.Name ?? character.Name;
                character.HitPoints = updatedCharacter.HitPoints ?? character.HitPoints;
                character.Strength = updatedCharacter.Strength ?? character.Strength;
                character.Defense = updatedCharacter.Defense ?? character.Defense;
                character.Intelligence = updatedCharacter.Intelligence ?? character.Intelligence;
                character.Class = updatedCharacter.Class ?? character.Class;
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(character);
                return serviceResponse;
            }
            catch (ArgumentNullException ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "ArgumentNullException: " + ex.Message;
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
