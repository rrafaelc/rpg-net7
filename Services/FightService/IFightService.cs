using rpg.Dtos.Fight;
using rpg.Models;

namespace rpg.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackRequestDto request);
        Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackRequestDto request);
    }
}
