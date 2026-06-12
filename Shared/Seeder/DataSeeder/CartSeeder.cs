using Bogus;
using Food_Market_BE.Modules.CartModule.Models;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class CartSeeder : IDataSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Cart> _cartCollection;

        public CartSeeder(IMongoDatabase database)
        {
            _database = database;
            // Giả sử collection của bạn tên là CartItems hoặc Carts
            _cartCollection = database.GetCollection<Cart>("Cart");
        }

        // Ưu tiên 5: Phải có Sản phẩm và User thì mới cho vào giỏ được
        public int Priority => 5;

        public async Task SeedAsync()
        {
            if (await _cartCollection.CountDocumentsAsync(FilterDefinition<Cart>.Empty) > 0) return;

            Console.WriteLine("[CartModule] Đang lấy ID User và Product để tạo Giỏ hàng...");

            // 1. Lấy danh sách AccountId là "User" (Khách hàng)
            var accountCollection = _database.GetCollection<BsonDocument>("Accounts");
            var userIds = await accountCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Role", "User"))
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            // 2. Lấy danh sách ProductId đang bán
            var productCollection = _database.GetCollection<BsonDocument>("Products");
            var productIds = await productCollection
                .Find(new BsonDocument())
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            if (!userIds.Any() || !productIds.Any())
            {
                Console.WriteLine("[CartModule] Thiếu User hoặc Product. Bỏ qua tạo Giỏ hàng.");
                return;
            }

            // 3. Sinh dữ liệu: Chọn ngẫu nhiên user và product bỏ vào giỏ
            int totalCartItems = Random.Shared.Next(500, 1000);
            var faker = new Faker<Cart>("vi")
                .RuleFor(c => c.UserId, f => f.PickRandom(userIds))
                .RuleFor(c => c.ProductId, f => f.PickRandom(productIds))
                .RuleFor(c => c.Quantity, f => f.Random.Int(1, 5)) // Mua từ 1 đến 5 sản phẩm
                .RuleFor(c => c.AddedAt, f => f.Date.Recent(15)); // Thêm vào giỏ trong 15 ngày gần đây

            var fakeCartItems = faker.Generate(totalCartItems);

            await _cartCollection.InsertManyAsync(fakeCartItems);
            Console.WriteLine($"[CartModule] Đã tạo thành công {totalCartItems} sản phẩm trong các Giỏ hàng!");
        }
    }
}
