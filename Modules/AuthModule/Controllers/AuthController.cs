using Food_Market_BE.Modules.AuthModule.DTOs;
using Food_Market_BE.Modules.AuthModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Modules.AuthModule.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            await _service.RegisterAsync(req);
            return Ok(new { message = "Register success" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            return Ok(await _service.LoginAsync(req));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest req)
        {
            return Ok(await _service.RefreshAsync(req.RefreshToken));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> Forgot(ForgotPasswordRequest req)
        {
            await _service.ForgotPasswordAsync(req.Email);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> Reset(ResetPasswordRequest req)
        {
            await _service.ResetPasswordAsync(req);
            return Ok();
        }
    }
}
