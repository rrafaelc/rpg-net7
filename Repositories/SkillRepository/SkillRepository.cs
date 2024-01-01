using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Models;

namespace rpg.Repositories.SkillRepository
{
    public class SkillRepository : ISkillRepository
    {
        private readonly DataContext _context;
        public SkillRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Skill> AddSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<List<Skill>> Find()
        {
            return await _context.Skills.ToListAsync();
        }

        public async Task<Skill?> FindSkillById(int id)
        {
            return await _context.Skills.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
