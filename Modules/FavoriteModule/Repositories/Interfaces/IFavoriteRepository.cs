using Food_Market_BE.Modules.FavoriteModule.Models;

namespace Food_Market_BE.Modules.FavoriteModule.Repositories.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<List<Favorite>> GetFavoritesByUserAsync(string userId);
        Task<Favorite?> GetFavoriteAsync(string userId, string productId);
        Task AddFavoriteAsync(Favorite favorite);
        Task<bool> RemoveFavoriteAsync(string userId, string productId);
        Task<bool> ExistsAsync(string userId, string productId);
    }
}
