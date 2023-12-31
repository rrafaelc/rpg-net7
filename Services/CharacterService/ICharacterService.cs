using rpg.Dtos.Character;
using rpg.Models;

namespace rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<CharacterResponseDto>>> GetAllCharacters();
        Task<ServiceResponse<CharacterResponseDto>> GetCharacterById(int id);
        Task<ServiceResponse<CharacterResponseDto>> AddCharacter(AddCharacterRequestDto newCharacter);
        Task<ServiceResponse<CharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter);
    }
}
