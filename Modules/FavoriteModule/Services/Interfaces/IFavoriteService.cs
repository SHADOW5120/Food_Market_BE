using Food_Market_BE.Modules.FavoriteModule.DTOs;

namespace Food_Market_BE.Modules.FavoriteModule.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<List<FavoriteItemDto>> GetUserFavoritesAsync(string userId);
        Task<FavoriteItemDto> AddToFavoritesAsync(string userId, string productId);
        Task<bool> RemoveFromFavoritesAsync(string userId, string productId);
    }
}
