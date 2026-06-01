using Food_Market_BE.Modules.FavoriteModule.DTOs;
using Food_Market_BE.Modules.FavoriteModule.Helpers;
using Food_Market_BE.Modules.FavoriteModule.Models;
using Food_Market_BE.Modules.FavoriteModule.Repositories.Interfaces;
using Food_Market_BE.Modules.FavoriteModule.Services.Interfaces;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.ProductModule.Repositories.Interfaces;

namespace Food_Market_BE.Modules.FavoriteModule.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        //private readonly IMongoCollection<Product> _products;
        private readonly IProductRepository _productRepository;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            //MongoDbContext database,
            IProductRepository productRespository)
        {
            _favoriteRepository = favoriteRepository;
            //_products = database.GetCollection<Product>("Products");
            _productRepository = productRespository;
        }

        public async Task<List<FavoriteItemDto>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await _favoriteRepository.GetFavoritesByUserAsync(userId);

            favorites = FavoriteHelper.SortNewestFirst(favorites);

            var result = new List<FavoriteItemDto>();

            foreach (var favorite in favorites)
            {
                var product = await _productRepository.GetByIdAsync(favorite.ProductId);
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
                throw new ArgumentException("ProductId is required.", nameof(productId));

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException("Product does not exist.");

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
                throw new ArgumentException("ProductId is required.", nameof(productId));

            return await _favoriteRepository.RemoveFavoriteAsync(userId, productId);
        }

        private string? GetProductImage(Product product)
        {
            if (product == null) return null;

            if (product.Images != null && product.Images.Any())
            {
                return product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                    ?? product.Images.FirstOrDefault()?.ImageUrl;
            }

            return null;
        }
    }
}
