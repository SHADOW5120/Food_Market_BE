using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Helpers;
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
        //private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<BsonDocument> _reviews;
        //private readonly IMongoCollection<BsonDocument> _categories;

        public StoreRepository(MongoDbContext database)
        {
            _stores = database.GetCollection<Store>("Stores");
            //_products = database.GetCollection<Product>("Products");
            _reviews = database.GetCollection<BsonDocument>("Reviews");
            //_categories = database.GetCollection<BsonDocument>("Categories");
        }

        public async Task<List<Store>> GetAllAsync()
        {
            return await _stores.Find(_ => true).ToListAsync();
        }

        public async Task<Store?> GetStoreByIdAsync(string storeId)
        {
            return await _stores.Find(x => x.Id == storeId && !x.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<List<Store>> GetStoreBySellerIdAsync(string ownerId)
        {
            return await _stores.Find(x => x.OwnerId == ownerId && !x.IsDeleted).ToListAsync();
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

        public async Task<List<Store>> SearchAsync(GetStoreQueryDto query)
        {
            var filter = StoreSearchHelper.BuildFilter(query);
            var sort = StoreSearchHelper.BuildSort(query);
            var paging = StoreSearchHelper.BuildPaging(query);

            return await _stores
                .Find(filter)
                .Sort(sort)
                .Skip(paging.Skip)
                .Limit(paging.Limit)
                .ToListAsync();
        }
    }
}
