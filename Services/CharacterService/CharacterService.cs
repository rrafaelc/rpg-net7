using rpg.Models;

namespace rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{ Id = 1, Name = "Sam"},
        };

        public async Task<ServiceResponse<Character>> AddCharacter(Character character)
        {
            var serviceResponse = new ServiceResponse<Character>();
            character.Id = characters.Last().Id + 1;
            characters.Add(character);
            serviceResponse.Data = character;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<Character>>
            {
                Data = characters
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<Character>> GetCharacterById(int id)
        {
            var character = characters.FirstOrDefault(x => x.Id == id);

            var serviceResponse = new ServiceResponse<Character>();
            serviceResponse.Data = character;
            return serviceResponse;
        }
    }
}
