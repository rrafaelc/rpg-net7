using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.Dtos.Fight;
using rpg.Models;
using rpg.Services.FightService;

namespace rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }

        [HttpPost("Weapon")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackRequestDto request)
        {
            var response = await _fightService.WeaponAttack(request);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackRequestDto request)
        {
            var response = await _fightService.SkillAttack(request);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FightResultDto>>> Fight(FightRequestDto request)
        {
            var response = await _fightService.Fight(request);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighScore()
        {
            var response = await _fightService.GetHighScore();
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
