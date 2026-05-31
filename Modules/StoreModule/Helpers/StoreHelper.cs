using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Models;

namespace Food_Market_BE.Modules.StoreModule.Helpers
{
    public static class StoreHelper
    {
        public static string TruncateDescription(string? description, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(description))
                return string.Empty;

            if (description.Length <= maxLength)
                return description;

            return description.Substring(0, maxLength) + "...";
        }

        public static bool IsOwner(Store store, string userId)
        {
            return store.OwnerId == userId;
        }

        public static StoreResponse ToStoreResponse(Store store)
        {
            return new StoreResponse
            {
                Id = store.Id,
                Name = store.Name,
                Description = store.Description,
                LogoUrl = store.LogoUrl,
                BannerUrl = store.BannerUrl,
                Rating = store.Rating,
                IsOpen = store.IsOpen
            };
        }

        public static StoreResponse ToStoreListItemDto(Store store)
        {
            return new StoreResponse
            {
                Id = store.Id,
                Name = store.Name,
                LogoUrl = store.LogoUrl,
                Rating = store.Rating,
                Description = TruncateDescription(store.Description),
                //TotalProducts = totalProducts,
                IsOpen = store.IsOpen
            };
        }

    }
}
