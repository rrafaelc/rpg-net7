using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.Dtos.Character;
using rpg.Dtos.Weapon;
using rpg.Models;
using rpg.Services.WeaponService;

namespace rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<CharacterResponseDto>>> Post(AddWeaponRequestDto request)
        {
            var response = await _weaponService.AddWeapon(request);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
