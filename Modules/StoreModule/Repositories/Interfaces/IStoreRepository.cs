using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Models;

namespace Food_Market_BE.Modules.StoreModule.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        Task<(List<Store> Stores, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            string? search,
            double? minRating);

        Task<Store?> GetStoreByIdAsync(string storeId);
        Task<Store?> GetStoreByOwnerIdAsync(string ownerId);

        Task CreateStoreAsync(Store store);
        Task UpdateStoreAsync(Store store);
        Task DeleteStoreAsync(string storeId);

        Task<List<StoreProductDto>> GetStoreProductsAsync(string storeId);
        Task<int> CountProductsByStoreIdAsync(string storeId);

        Task<double> CalculateStoreRatingAsync(string storeId);
    }
}
