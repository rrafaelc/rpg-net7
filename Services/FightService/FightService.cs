using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Dtos.Fight;
using rpg.Models;
using rpg.Repositories.CharacterRepository;

namespace rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly ICharacterRepository _characterRepository;
        private readonly IMapper _mapper;
        public FightService(DataContext context, ICharacterRepository characterRepository, IMapper mapper)
        {
            _context = context;
            _characterRepository = characterRepository;
            _mapper = mapper;
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
                int damage = DoWeaponAttack(attacker, opponent);

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

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackRequestDto request)
        {
            var serviceResponse = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _characterRepository.FindCharacterById(request.AttackerId)
                ?? throw new Exception("Attacker not found");
                var opponent = await _characterRepository.FindCharacterById(request.OpponentId)
                ?? throw new Exception("Opponent not found");

                if (attacker.Skills is null) throw new Exception($"{attacker.Name} doesn't have any skills!");
                var skill = attacker.Skills.FirstOrDefault(x => x.Id == request.SkillId)
                ?? throw new Exception($"{attacker.Name} doesn't know that skill!");
                int damage = DoSkillAttack(attacker, opponent, skill);

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

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var serviceResponse = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };

            try
            {
                var characters = await _context.Characters
                    .Include(x => x.Weapon)
                    .Include(x => x.Skills)
                    .Where(x => request.CharacterIds.Contains(x.Id))
                    .ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (var attacker in characters)
                    {
                        var opponents = characters.Where(x => x.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon && attacker.Weapon is not null)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else if (!useWeapon && attacker.Skills is not null && attacker.Skills.Count > 0)
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }
                        else
                        {
                            serviceResponse.Data.Log
                                .Add($"{attacker.Name} wasn't able to attack!");
                            continue;
                        }

                        serviceResponse.Data.Log
                                .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage!");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            serviceResponse.Data!.Log.Add($"{opponent.Name} has been defeated!");
                            serviceResponse.Data!.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(x =>
                {
                    x.Fights++;
                    x.HitPoints = 100;
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            var characters = await _context.Characters
                .Where(x => x.Fights > 0)
                .OrderByDescending(x => x.Victories)
                .ThenBy(x => x.Defeats)
                .ToListAsync();

            var serviceResponse = new ServiceResponse<List<HighScoreDto>>
            {
                Data = characters.Select(x => _mapper.Map<HighScoreDto>(x)).ToList()
            };

            return serviceResponse;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            if (attacker.Weapon is null) throw new Exception("Attacker has no weapon!");

            int damage = attacker.Weapon.Damage + new Random().Next(attacker.Strength);
            damage -= new Random().Next(opponent.Defeats);

            if (damage > 0) opponent.HitPoints -= damage;
            return damage;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            int damage = skill.Damage + new Random().Next(attacker.Intelligence);
            damage -= new Random().Next(opponent.Defeats);

            if (damage > 0) opponent.HitPoints -= damage;
            return damage;
        }
    }
}
