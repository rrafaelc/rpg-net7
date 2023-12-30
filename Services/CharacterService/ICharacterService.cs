using rpg.Dtos.Character;
using rpg.Models;

namespace rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<CharacterResponseDto>>> GetAllCharacters();
        Task<ServiceResponse<CharacterResponseDto>> GetCharacterById(int id);
        Task<ServiceResponse<List<CharacterResponseDto>>> AddCharacter(AddCharacterRequestDto addCharacterRequestDto);
    }
}
