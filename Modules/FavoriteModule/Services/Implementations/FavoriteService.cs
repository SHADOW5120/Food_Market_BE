using Food_Market_BE.Modules.FavoriteModule.DTOs;
using Food_Market_BE.Modules.FavoriteModule.Helpers;
using Food_Market_BE.Modules.FavoriteModule.Models;
using Food_Market_BE.Modules.FavoriteModule.Repositories.Interfaces;
using Food_Market_BE.Modules.FavoriteModule.Services.Interfaces;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.FavoriteModule.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IMongoCollection<Product> _products;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            MongoDbContext database)
        {
            _favoriteRepository = favoriteRepository;
            _products = database.GetCollection<Product>("Products");
        }

        public async Task<List<FavoriteItemDto>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await _favoriteRepository.GetFavoritesByUserAsync(userId);

            favorites = FavoriteHelper.SortNewestFirst(favorites);

            var result = new List<FavoriteItemDto>();

            foreach (var favorite in favorites)
            {
                var product = await _products.Find(x => x.Id == favorite.ProductId).FirstOrDefaultAsync();
                if (product == null) continue;

                result.Add(new FavoriteItemDto
                {
                    FavoriteId = favorite.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = GetProductImage(product),
                    Price = product.Price,
                    CreatedDate = favorite.CreatedDate
                });
            }

            return result;
        }

        public async Task<FavoriteItemDto> AddToFavoritesAsync(string userId, string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                throw new Exception("ProductId is required.");

            var product = await _products.Find(x => x.Id == productId).FirstOrDefaultAsync();
            if (product == null)
                throw new Exception("Product does not exist.");

            var existingFavorite = await _favoriteRepository.GetFavoriteAsync(userId, productId);

            if (existingFavorite != null)
            {
                return new FavoriteItemDto
                {
                    FavoriteId = existingFavorite.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = GetProductImage(product),
                    Price = product.Price,
                    CreatedDate = existingFavorite.CreatedDate
                };
            }

            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId,
                CreatedDate = DateTime.UtcNow
            };

            await _favoriteRepository.AddFavoriteAsync(favorite);

            return new FavoriteItemDto
            {
                FavoriteId = favorite.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = GetProductImage(product),
                Price = product.Price,
                CreatedDate = favorite.CreatedDate
            };
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                throw new Exception("ProductId is required.");

            return await _favoriteRepository.RemoveFavoriteAsync(userId, productId);
        }

        private string? GetProductImage(Product product)
        {
            var imageProperty = product.GetType().GetProperty("ImageUrl");
            if (imageProperty != null)
            {
                return imageProperty.GetValue(product)?.ToString();
            }

            var imagesProperty = product.GetType().GetProperty("Images");
            if (imagesProperty != null)
            {
                var images = imagesProperty.GetValue(product) as IEnumerable<string>;
                return images?.FirstOrDefault();
            }

            return null;
        }
    }
}
