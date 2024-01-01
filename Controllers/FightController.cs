using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.Dtos.Fight;
using rpg.Models;
using rpg.Services.FightService;

namespace rpg.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> Post(WeaponAttackRequestDto request)
        {
            var response = await _fightService.WeaponAttack(request);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}