using Food_Market_BE.Modules.AuthModule.DTOs;
using Food_Market_BE.Modules.AuthModule.Services.Interfaces;
using Food_Market_BE.Shared.Responses;
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
            return Ok(ApiResponse<string>.SuccessResponse(null, "Register success"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var result = await _service.LoginAsync(req);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest req)
        {
            var result = await _service.RefreshAsync(req.RefreshToken);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(result));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> Forgot(ForgotPasswordRequest req)
        {
            await _service.ForgotPasswordAsync(req.Email);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Password reset token sent if email exists"));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> Reset(ResetPasswordRequest req)
        {
            await _service.ResetPasswordAsync(req);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Password reset success"));
        }
    }
}