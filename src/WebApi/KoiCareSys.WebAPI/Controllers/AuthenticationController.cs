using KoiCareSys.Data.DTO;
using KoiCareSys.Service.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KoiCareSys.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthenticationController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var token = _tokenService.GenerateToken(loginDto);
            return Ok(new { token });
        }

    }
}
