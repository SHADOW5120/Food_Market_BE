using Food_Market_BE.Modules.AuthModule.Models;

namespace Food_Market_BE.Modules.AuthModule.Repositories.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task CreateAsync(PasswordResetToken token);
        Task<PasswordResetToken> GetAsync(string token);
        Task MarkUsedAsync(string token);
    }
}
