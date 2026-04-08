using Food_Market_BE.Modules.ProductModule.Models;
using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Models;
using Food_Market_BE.Modules.StoreModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.StoreModule.Repositories.Implementations
{
    public class StoreRepository : IStoreRepository
    {
        private readonly IMongoCollection<Store> _stores;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<BsonDocument> _reviews;
        private readonly IMongoCollection<BsonDocument> _categories;

        public StoreRepository(MongoDbContext database)
        {
            _stores = database.GetCollection<Store>("Stores");
            _products = database.GetCollection<Product>("Products");
            _reviews = database.GetCollection<BsonDocument>("Reviews");
            _categories = database.GetCollection<BsonDocument>("Categories");
        }

        public async Task<(List<Store> Stores, int TotalCount)> GetStoresAsync(
            int page,
            int pageSize,
            string? search,
            double? minRating)
        {
            var filterBuilder = Builders<Store>.Filter;
            var filter = filterBuilder.Eq(x => x.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter &= filterBuilder.Regex(x => x.Name, new BsonRegularExpression(search, "i"));
            }

            if (minRating.HasValue)
            {
                filter &= filterBuilder.Gte(x => x.Rating, minRating.Value);
            }

            var totalCount = (int)await _stores.CountDocumentsAsync(filter);

            var stores = await _stores.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (stores, totalCount);
        }

        public async Task<Store?> GetStoreByIdAsync(string storeId)
        {
            return await _stores.Find(x => x.Id == storeId && !x.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Store?> GetStoreByOwnerIdAsync(string ownerId)
        {
            return await _stores.Find(x => x.OwnerId == ownerId && !x.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task CreateStoreAsync(Store store)
        {
            await _stores.InsertOneAsync(store);
        }

        public async Task UpdateStoreAsync(Store store)
        {
            await _stores.ReplaceOneAsync(x => x.Id == store.Id, store);
        }

        public async Task DeleteStoreAsync(string storeId)
        {
            var update = Builders<Store>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            await _stores.UpdateOneAsync(x => x.Id == storeId, update);
        }

        public async Task<List<StoreProductDto>> GetStoreProductsAsync(string storeId)
        {
            var products = await _products
                .Find(p => p.StoreId == storeId && !p.IsDeleted)
                .ToListAsync();

            var categoryIds = products
                .Where(x => !string.IsNullOrWhiteSpace(x.CategoryId))
                .Select(x => x.CategoryId!)
                .Distinct()
                .ToList();

            var categories = await _categories
                .Find(Builders<BsonDocument>.Filter.In("_id", categoryIds.Select(ObjectId.Parse)))
                .ToListAsync();

            var categoryMap = categories.ToDictionary(
                c => c["_id"].ToString(),
                c => c.Contains("Name") ? c["Name"].AsString : ""
            );

            return products.Select(p => new StoreProductDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Price = p.Price,
                Image = p.ImageUrl,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = !string.IsNullOrWhiteSpace(p.CategoryId) && categoryMap.ContainsKey(p.CategoryId)
                    ? categoryMap[p.CategoryId]
                    : null
            }).ToList();
        }

        public async Task<int> CountProductsByStoreIdAsync(string storeId)
        {
            return (int)await _products.CountDocumentsAsync(x => x.StoreId == storeId && !x.IsDeleted);
        }

        public async Task<double> CalculateStoreRatingAsync(string storeId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("StoreId", storeId);

            var reviews = await _reviews.Find(filter).ToListAsync();

            if (!reviews.Any()) return 0;

            var ratings = reviews
                .Where(r => r.Contains("Rating"))
                .Select(r => r["Rating"].ToDouble())
                .ToList();

            if (!ratings.Any()) return 0;

            return Math.Round(ratings.Average(), 1);
        }
    }
}
