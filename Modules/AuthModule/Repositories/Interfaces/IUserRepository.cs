using Food_Market_BE.Modules.AuthModule.Models;

namespace Food_Market_BE.Modules.AuthModule.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(string id);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
    }
}
