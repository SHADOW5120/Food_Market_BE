using Bogus;
using Food_Market_BE.Modules.StoreModule.Models;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class StoreSeeder : IDataSeeder
    {
        private readonly IMongoCollection<Store> _storeCollection;
        private readonly MongoDbContext _database;

        public StoreSeeder(MongoDbContext database)
        {
            _database = database;
            _storeCollection = database.GetCollection<Store>("Stores");
        }

        public int Priority => 3;

        public async Task SeedAsync()
        {
            if (await _storeCollection.CountDocumentsAsync(FilterDefinition<Store>.Empty) > 0) return;

            Console.WriteLine("[StoreModule] Đang tạo dữ liệu Cửa hàng...");

            // Lấy danh sách user id để gán owner
            var userCollection = _database.GetCollection<BsonDocument>("Users");
            var userIds = await userCollection.Find(new BsonDocument()).Project(b => b["_id"].ToString()).ToListAsync();

            if (!userIds.Any())
            {
                Console.WriteLine("[StoreModule] Không có User để gán Owner cho Store. Bỏ qua tạo Store.");
                return;
            }

            var faker = new Faker<Store>("vi")
                .RuleFor(s => s.Id, f => ObjectId.GenerateNewId().ToString())
                .RuleFor(s => s.Name, f => f.Company.CompanyName())
                .RuleFor(s => s.Description, f => f.Company.CatchPhrase())
                .RuleFor(s => s.LogoUrl, f => f.Image.PicsumUrl())
                .RuleFor(s => s.BannerUrl, f => f.Image.PicsumUrl())
                .RuleFor(s => s.OwnerId, f => f.PickRandom(userIds))
                .RuleFor(s => s.Rating, f => Math.Round(f.Random.Double(3.0, 5.0), 2))
                .RuleFor(s => s.IsOpen, f => f.Random.Bool(80))
                .RuleFor(s => s.CreatedAt, f => f.Date.Past(2));

            var stores = faker.Generate(50);

            await _storeCollection.InsertManyAsync(stores);
            Console.WriteLine($"[StoreModule] Đã tạo thành công {stores.Count} Cửa hàng.");
        }
    }
}
