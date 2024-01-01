using rpg.Models;

namespace rpg.Repositories.CharacterRepository
{
    public interface ICharacterRepository
    {
        Task<List<Character>> FindCharacters(int? userId);
        Task<Character?> FindCharacterById(int id);
        Task<Character> AddCharacter(Character character);
        Task<Character> UpdateCharacter(Character character);
        Task DeleteCharacter(Character character);
    }
}
