using Bogus;
using Food_Market_BE.Modules.AuthModule.Models;
using Food_Market_BE.Modules.FavoriteModule.Models;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class FavoriteSeeder : IDataSeeder
    {
        private readonly MongoDbContext _database;
        private readonly IMongoCollection<Favorite> _favoriteCollection;

        public FavoriteSeeder(MongoDbContext database)
        {
            _database = database;
            _favoriteCollection = database.GetCollection<Favorite>("Favorites");
        }

        public int Priority => 5;

        public async Task SeedAsync()
        {
            if (await _favoriteCollection.CountDocumentsAsync(FilterDefinition<Favorite>.Empty) > 0) return;

            var userCollection = _database.GetCollection<BsonDocument>("Users");
            var userIds = await userCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Role", UserRole.User))
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            var productCollection = _database.GetCollection<BsonDocument>("Products");
            var productIds = await productCollection
                .Find(new BsonDocument())
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            if (!userIds.Any() || !productIds.Any())
            {
                Console.WriteLine("[FavoriteModule] Thiếu User hoặc Product. Bỏ qua tạo Favorite.");
                return;
            }

            int totalFavorites = Random.Shared.Next(800, 1500);
            var faker = new Faker("vi");

            var favorites = Enumerable.Range(0, totalFavorites)
                .Select(_ => new Favorite
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = faker.PickRandom(userIds),
                    ProductId = faker.PickRandom(productIds),
                    CreatedDate = faker.Date.Recent(30)
                })
                .ToList();

            await _favoriteCollection.InsertManyAsync(favorites);
            Console.WriteLine($"[FavoriteModule] Đã tạo thành công {favorites.Count} lượt Yêu thích!");
        }
    }
}
