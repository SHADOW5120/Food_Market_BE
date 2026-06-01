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

        // =========================
        // LIST STORES (PUBLIC)
        // =========================
        public async Task<PagedStoreResponse> GetStoresAsync(GetStoreQueryDto query)
        {
            var stores = await _storeRepository.SearchAsync(query);

            return MapPaged(stores, query);
        }

        // =========================
        // STORE BY SELLER
        // =========================
        public async Task<PagedStoreResponse> GetStoreBySellerIdAsync(string sellerId, GetStoreQueryDto query)
        {
            var stores = await _storeRepository.GetStoreBySellerIdAsync(sellerId);

            var filtered = ApplyFilters(stores, query);

            return MapPaged(filtered, query);
        }

        // =========================
        // DETAIL
        // =========================
        public async Task<StoreResponse?> GetStoreByIdAsync(string storeId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);

            if (store == null || store.IsDeleted)
                return null;

            var rating = await _storeRepository.CalculateStoreRatingAsync(storeId);

            if (store.Rating != rating)
            {
                store.Rating = rating;
                store.UpdatedAt = DateTime.UtcNow;
                await _storeRepository.UpdateStoreAsync(store);
            }

            return new StoreResponse
            {
                Id = store.Id,
                Name = store.Name,
                LogoUrl = store.LogoUrl,
                BannerUrl = store.BannerUrl,
                Description = store.Description,
                Rating = store.Rating,
                IsOpen = store.IsOpen
            };
        }

        // =========================
        // CREATE
        // =========================
        public async Task<StoreResponse> CreateStoreAsync(CreateStoreRequest request, string userId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

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

        // =========================
        // UPDATE
        // =========================
        public async Task<StoreResponse> UpdateStoreAsync(string storeId, UpdateStoreRequest request, string userId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);

            if (store == null || store.IsDeleted)
                throw new InvalidOperationException("Store not found");

            if (store.OwnerId != userId)
                throw new UnauthorizedAccessException("You do not have permission");

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

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteStoreAsync(string storeId, string userId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);

            if (store == null || store.IsDeleted)
                return false;

            if (store.OwnerId != userId)
                throw new UnauthorizedAccessException("You do not have permission");

            await _storeRepository.DeleteStoreAsync(storeId);
            return true;
        }

        // =========================
        // PRIVATE HELPERS
        // =========================
        private static List<Store> ApplyFilters(List<Store> stores, GetStoreQueryDto query)
        {
            return stores
                .Where(x => !x.IsDeleted)
                .Where(x => string.IsNullOrWhiteSpace(query.Search)
                            || x.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase))
                .Where(x => !query.MinRating.HasValue
                            || x.Rating >= query.MinRating.Value)
                .ToList();
        }

        private static PagedStoreResponse MapPaged(List<Store> stores, GetStoreQueryDto query)
        {
            var total = stores.Count;

            var items = stores
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(StoreHelper.ToStoreListItemDto)
                .ToList();

            return new PagedStoreResponse
            {
                Items = items,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}