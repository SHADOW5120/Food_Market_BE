using Bogus;
using Food_Market_BE.Modules.FavoriteModule.Models;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class FavoriteSeeder : IDataSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Favorite> _favoriteCollection;

        public FavoriteSeeder(IMongoDatabase database)
        {
            _database = database;
            _favoriteCollection = database.GetCollection<Favorite>("Favorites");
        }

        public int Priority => 5;

        public async Task SeedAsync()
        {
            if (await _favoriteCollection.CountDocumentsAsync(FilterDefinition<Favorite>.Empty) > 0) return;

            // 1. Lấy dữ liệu User và Product
            var accountCollection = _database.GetCollection<BsonDocument>("Accounts");
            var userIds = await accountCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Role", "User"))
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            var productCollection = _database.GetCollection<BsonDocument>("Products");
            var productIds = await productCollection
                .Find(new BsonDocument())
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            if (!userIds.Any() || !productIds.Any()) return;

            // 2. Fake dữ liệu thả tim
            int totalFavorites = Random.Shared.Next(800, 1500);
            var faker = new Faker<Favorite>("vi")
                .RuleFor(f => f.AccountId, f => f.PickRandom(userIds))
                .RuleFor(f => f.ProductId, f => f.PickRandom(productIds))
                .RuleFor(f => f.LikedAt, f => f.Date.Recent(30));

            var fakeFavorites = faker.Generate(totalFavorites);

            await _favoriteCollection.InsertManyAsync(fakeFavorites);
            Console.WriteLine($"[FavoriteModule] Đã tạo thành công {totalFavorites} lượt Yêu thích!");
        }
    }
}
