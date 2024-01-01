using rpg.Models;

namespace rpg.Repositories.SkillRepository
{
    public interface ISkillRepository
    {
        Task<List<Skill>> Find();
        Task<Skill?> FindSkillById(int id);
        Task<Skill> AddSkill(Skill skill);
    }
}
