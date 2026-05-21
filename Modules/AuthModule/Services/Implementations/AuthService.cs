using Food_Market_BE.Modules.AuthModule.Models;
using Food_Market_BE.Modules.AuthModule.DTOs;
using Food_Market_BE.Modules.AuthModule.Repositories.Interfaces;
using Food_Market_BE.Modules.AuthModule.Services.Interfaces;
using Food_Market_BE.Shared.Exceptions;
using Food_Market_BE.Modules.AuthModule.Helpers;

namespace Food_Market_BE.Modules.AuthModule.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IPasswordResetRepository _resetRepo;
        private readonly PasswordHasher _hasher;
        private readonly JwtHelper _jwt;
        private readonly TokenGenerator _token;

        public AuthService(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshRepo,
            IPasswordResetRepository resetRepo,
            PasswordHasher hasher,
            JwtHelper jwt,
            TokenGenerator token)
        {
            _userRepo = userRepo;
            _refreshRepo = refreshRepo;
            _resetRepo = resetRepo;
            _hasher = hasher;
            _jwt = jwt;
            _token = token;
        }

        public async Task RegisterAsync(RegisterRequest req)
        {
            var exist = await _userRepo.GetByEmailAsync(req.Email);
            if (exist != null)
                throw new AppException("Email already exists", 400);

            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = _hasher.Hash(req.Password),
                Role = UserRole.User,
                IsActive = true,
            };

            await _userRepo.CreateAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req)
        {
            var user = await _userRepo.GetByEmailAsync(req.Email);

            if (user == null || !_hasher.Verify(req.Password, user.PasswordHash))
                throw new AppException("Invalid credentials", 401);

            var access = _jwt.GenerateToken(user.Id, user.Email, user.Role);
            var refresh = _token.Generate();

            await _refreshRepo.CreateAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refresh,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            return new AuthResponse
            {
                AccessToken = access,
                RefreshToken = refresh,
                User = new { user.Id, user.Username, user.Email }
            };
        }

        public async Task<AuthResponse> RefreshAsync(string refreshToken)
        {
            var token = await _refreshRepo.GetAsync(refreshToken);

            if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
                throw new AppException("Invalid refresh token", 401);

            var user = await _userRepo.GetByIdAsync(token.UserId);

            var access = _jwt.GenerateToken(user.Id, user.Email, user.Role);

            return new AuthResponse
            {
                AccessToken = access,
                RefreshToken = refreshToken,
                User = new { user.Id, user.Email }
            };
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return; // không báo lỗi để tránh lộ email

            var token = _token.Generate();

            await _resetRepo.CreateAsync(new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });

            // TODO: Gửi email chứa token (bạn thêm phần này)
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest req)
        {
            var token = await _resetRepo.GetAsync(req.Token);

            if (token == null || token.Used || token.ExpiresAt < DateTime.UtcNow)
                throw new AppException("Invalid or expired token", 400);

            var user = await _userRepo.GetByIdAsync(token.UserId);

            user.PasswordHash = _hasher.Hash(req.NewPassword);

            await _userRepo.UpdateAsync(user);
            await _resetRepo.MarkUsedAsync(req.Token);
        }
    }
}