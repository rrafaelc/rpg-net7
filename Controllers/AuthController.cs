using Microsoft.AspNetCore.Mvc;
using rpg.Dtos.User;
using rpg.Models;
using rpg.Services.AuthService;

namespace rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginRequestDto request)
        {
            var response = await _authService.Login(request.Username, request.Password);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterRequestDto request)
        {
            var response = await _authService.Register(
                new User { Username = request.Username }, request.Password
            );
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
