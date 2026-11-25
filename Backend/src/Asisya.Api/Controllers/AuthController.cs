using Asisya.Application.Auth.Dto;
using Asisya.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asisya.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct = default)
        {
            var result = await _auth.LoginAsync(request, ct);
            return result is null
                  ? Unauthorized(new { message = "Credenciales inválidas" })
                  : Ok(result);
        }


    }
}
