using rpg.Dtos.Fight;
using rpg.Models;
using rpg.Repositories.CharacterRepository;

namespace rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly ICharacterRepository _characterRepository;
        public FightService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackRequestDto request)
        {
            var serviceResponse = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _characterRepository.FindCharacterById(request.AttackerId)
                ?? throw new Exception("Attacker not found");
                var opponent = await _characterRepository.FindCharacterById(request.OpponentId)
                ?? throw new Exception("Opponent not found");
                if (attacker.Weapon is null) throw new Exception("Attacker's Weapon does not exist");

                int damage = attacker.Weapon.Damage + new Random().Next(attacker.Strength);
                damage -= new Random().Next(opponent.Defeats);

                if (damage > 0) opponent.HitPoints -= damage;

                if (opponent.HitPoints <= 0)
                    serviceResponse.Message = $"{opponent.Name} has been defeated!";

                await _characterRepository.UpdateCharacter(opponent);

                serviceResponse.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
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
