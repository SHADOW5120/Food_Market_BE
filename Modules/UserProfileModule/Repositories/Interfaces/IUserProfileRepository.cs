using Food_Market_BE.Modules.UserProfileModule.Models;

namespace Food_Market_BE.Modules.UserProfileModule.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetByUserIdAsync(string userId);
        Task CreateAsync(UserProfile profile);
        Task UpdateAsync(UserProfile profile);
    }
}
