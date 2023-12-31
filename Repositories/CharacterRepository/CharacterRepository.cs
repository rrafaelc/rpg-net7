using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Models;

namespace rpg.Repositories.CharacterRepository
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly DataContext _context;
        public CharacterRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Character>> FindCharacters(int? userId)
        {
            if (userId is null)
                return await _context.Characters
                .Include(x => x.User)
                .Include(x => x.Weapon)
                .Include(x => x.Skills)
                .ToListAsync();
            return await _context.Characters
            .Include(x => x.User)
            .Include(x => x.Weapon)
            .Include(x => x.Skills)
            .Where(x => x.User!.Id == userId).ToListAsync();
        }

        public async Task<Character?> FindCharacterById(int id)
        {
            return await _context.Characters
            .Include(x => x.User)
            .Include(x => x.Weapon)
            .Include(x => x.Skills)
            .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Character?> FindCharacterByIdAndUserId(int id, int userId)
        {
            // Para incluir o usuario ao x.User <- aqui, nao necessario aqui nesse metodo, mas eh uma dica
            // funciona parecido ao { relations: { user : true } } ou { eager : true } do typeorm
            // Weapon e Skills precisou
            return await _context.Characters
            .Include(x => x.User)
            .Include(x => x.Weapon)
            .Include(x => x.Skills)
            .FirstOrDefaultAsync(x => x.Id == id && x.User!.Id == userId);
        }

        public async Task<Character> AddCharacter(Character character)
        {
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task<Character> UpdateCharacter(Character character)
        {
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task DeleteCharacter(Character character)
        {
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
        }
    }
}
