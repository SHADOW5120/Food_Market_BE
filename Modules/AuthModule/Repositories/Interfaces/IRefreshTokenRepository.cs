using Food_Market_BE.Modules.AuthModule.Models;

namespace Food_Market_BE.Modules.AuthModule.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken token);
        Task<RefreshToken> GetAsync(string token);
        Task RevokeAsync(string token);
    }
}
