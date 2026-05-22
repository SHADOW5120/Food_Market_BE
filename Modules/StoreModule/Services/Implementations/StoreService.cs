using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Helpers;
using Food_Market_BE.Modules.StoreModule.Models;
using Food_Market_BE.Modules.StoreModule.Repositories.Interfaces;
using Food_Market_BE.Modules.StoreModule.Services.Interfaces;

namespace Food_Market_BE.Modules.StoreModule.Services.Implementations
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<PagedStoreResponse> GetStoresAsync(int page, int pageSize, string? search, double? minRating)
        {
            var (stores, totalCount) = await _storeRepository.GetAllAsync(page, pageSize, search, minRating);

            var items = new List<StoreListItemDto>();

            foreach (var store in stores)
            {
                var totalProducts = await _storeRepository.CountProductsByStoreIdAsync(store.Id);
                items.Add(StoreHelper.ToStoreListItemDto(store, totalProducts));
            }

            return new PagedStoreResponse
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<StoreResponse?> GetStoreDetailAsync(string storeId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);
            if (store == null) return null;

            var rating = await _storeRepository.CalculateStoreRatingAsync(storeId);
            if (store.Rating != rating)
            {
                store.Rating = rating;
                store.UpdatedAt = DateTime.UtcNow;
                await _storeRepository.UpdateStoreAsync(store);
            }

            var products = await _storeRepository.GetStoreProductsAsync(storeId);
            var categories = StoreHelper.ExtractCategories(products);

            return new StoreResponse
            {
                Id = store.Id,
                Name = store.Name,
                LogoUrl = store.LogoUrl,
                BannerUrl = store.BannerUrl,
                Description = store.Description,
                Rating = store.Rating,
                IsOpen = store.IsOpen,
                Categories = categories,
                Products = products
            };
        }

        public async Task<StoreResponse?> GetStoreBySellerIdAsync(string sellerId)
        {
            var store = await _storeRepository.GetStoreByOwnerIdAsync(sellerId);
            if (store == null) return null;

            var rating = await _storeRepository.CalculateStoreRatingAsync(store.Id);
            if (store.Rating != rating)
            {
                store.Rating = rating;
                store.UpdatedAt = DateTime.UtcNow;
                await _storeRepository.UpdateStoreAsync(store);
            }

            var products = await _storeRepository.GetStoreProductsAsync(store.Id);
            var categories = StoreHelper.ExtractCategories(products);

            return new StoreResponse
            {
                Id = store.Id,
                Name = store.Name,
                LogoUrl = store.LogoUrl,
                BannerUrl = store.BannerUrl,
                Description = store.Description,
                Rating = store.Rating,
                IsOpen = store.IsOpen,
                Categories = categories,
                Products = products
            };
        }

        public async Task<List<StoreProductDto>> GetStoreProductsAsync(string storeId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);
            if (store == null)
                throw new Exception("Store not found");

            return await _storeRepository.GetStoreProductsAsync(storeId);
        }

        public async Task<StoreResponse> CreateStoreAsync(CreateStoreRequest request, string userId)
        {
            var store = new Store
            {
                Name = request.Name,
                Description = request.Description,
                LogoUrl = request.LogoUrl,
                BannerUrl = request.BannerUrl,
                OwnerId = userId,
                Rating = 0,
                IsDeleted = false,
                IsOpen = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _storeRepository.CreateStoreAsync(store);

            return StoreHelper.ToStoreResponse(store);
        }

        public async Task<StoreResponse> UpdateStoreAsync(string storeId, UpdateStoreRequest request, string userId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);
            if (store == null)
                throw new Exception("Store not found");

            var isOwner = StoreHelper.IsOwner(store, userId);

            if (!isOwner)
                throw new Exception("You do not have permission to update this store");

            if (!string.IsNullOrWhiteSpace(request.Name))
                store.Name = request.Name;

            if (request.Description != null)
                store.Description = request.Description;

            if (request.LogoUrl != null)
                store.LogoUrl = request.LogoUrl;

            if (request.BannerUrl != null)
                store.BannerUrl = request.BannerUrl;

            if (request.IsOpen.HasValue)
                store.IsOpen = request.IsOpen.Value;

            store.Rating = await _storeRepository.CalculateStoreRatingAsync(store.Id);
            store.UpdatedAt = DateTime.UtcNow;

            await _storeRepository.UpdateStoreAsync(store);

            return StoreHelper.ToStoreResponse(store);
        }

        public async Task<bool> DeleteStoreAsync(string storeId, string userId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);
            if (store == null)
                throw new Exception("Store not found");

            var isOwner = StoreHelper.IsOwner(store, userId);

            if (!isOwner)
                throw new Exception("You do not have permission to delete this store");

            await _storeRepository.DeleteStoreAsync(storeId);
            return true;
        }
    }
}
