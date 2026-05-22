using Food_Market_BE.Modules.AuthModule.DTOs;

namespace Food_Market_BE.Modules.AuthModule.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest req);
        Task<AuthResponse> LoginAsync(LoginRequest req);
        Task<AuthResponse> RefreshAsync(string refreshToken);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordRequest req);
    }
}
