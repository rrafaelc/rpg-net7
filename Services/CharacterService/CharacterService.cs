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

        public async Task<ServiceResponse<List<CharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<CharacterResponseDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.Id = characters.Max(x => x.Id) + 1;
            characters.Add(character);
            serviceResponse.Data = characters.Select(x => _mapper.Map<CharacterResponseDto>(x)).ToList();
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
    }
}
