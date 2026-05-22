using Food_Market_BE.Modules.StoreModule.DTOs;

namespace Food_Market_BE.Modules.StoreModule.Services.Interfaces
{
    public interface IStoreService
    {
        Task<PagedStoreResponse> GetStoresAsync(int page, int pageSize, string? search, double? minRating);
        Task<StoreResponse?> GetStoreDetailAsync(string storeId);
        Task<StoreResponse?> GetStoreBySellerIdAsync(string sellerId);
        Task<List<StoreProductDto>> GetStoreProductsAsync(string storeId);

        Task<StoreResponse> CreateStoreAsync(CreateStoreRequest request, string userId);
        Task<StoreResponse> UpdateStoreAsync(string storeId, UpdateStoreRequest request, string userId);
        Task<bool> DeleteStoreAsync(string storeId, string userId);
    }
}
