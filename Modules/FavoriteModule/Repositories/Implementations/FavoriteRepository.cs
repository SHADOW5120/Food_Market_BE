using Food_Market_BE.Modules.FavoriteModule.Models;
using Food_Market_BE.Modules.FavoriteModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.FavoriteModule.Repositories.Implementations
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly IMongoCollection<Favorite> _favorites;

        public FavoriteRepository(MongoDbContext database)
        {
            _favorites = database.GetCollection<Favorite>("Favorites");
        }

        public async Task<List<Favorite>> GetFavoritesByUserAsync(string userId)
        {
            return await _favorites
                .Find(x => x.UserId == userId)
                .SortByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Favorite?> GetFavoriteAsync(string userId, string productId)
        {
            return await _favorites
                .Find(x => x.UserId == userId && x.ProductId == productId)
                .FirstOrDefaultAsync();
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            await _favorites.InsertOneAsync(favorite);
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, string productId)
        {
            var result = await _favorites.DeleteOneAsync(x => x.UserId == userId && x.ProductId == productId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsAsync(string userId, string productId)
        {
            return await _favorites.Find(x => x.UserId == userId && x.ProductId == productId).AnyAsync();
        }
    }
}
