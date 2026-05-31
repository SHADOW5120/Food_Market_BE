using Food_Market_BE.Modules.StoreModule.DTOs;

namespace Food_Market_BE.Modules.StoreModule.Services.Interfaces
{
    public interface IStoreService
    {
        Task<PagedStoreResponse> GetStoresAsync(GetStoreQueryDto query);
        Task<StoreResponse?> GetStoreByIdAsync(string storeId);
        Task<PagedStoreResponse> GetStoreBySellerIdAsync(string sellerId, GetStoreQueryDto query);

        Task<StoreResponse> CreateStoreAsync(CreateStoreRequest request, string userId);
        Task<StoreResponse> UpdateStoreAsync(string storeId, UpdateStoreRequest request, string userId);
        Task<bool> DeleteStoreAsync(string storeId, string userId);
    }
}
