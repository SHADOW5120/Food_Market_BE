using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Models;

namespace Food_Market_BE.Modules.StoreModule.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        Task<List<Store>> GetAllAsync();

        Task<Store?> GetStoreByIdAsync(string storeId);
        Task<List<Store>> GetStoreBySellerIdAsync(string ownerId);

        Task CreateStoreAsync(Store store);
        Task UpdateStoreAsync(Store store);
        Task DeleteStoreAsync(string storeId);

        Task<double> CalculateStoreRatingAsync(string storeId);

        Task<List<Store>> SearchAsync(GetStoreQueryDto query);
    }
}
