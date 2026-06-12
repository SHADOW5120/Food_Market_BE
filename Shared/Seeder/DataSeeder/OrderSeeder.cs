using Bogus;
using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class OrderSeeder : IDataSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Order> _orderCollection;

        public OrderSeeder(IMongoDatabase database)
        {
            _database = database;
            _orderCollection = database.GetCollection<Order>("Orders");
        }

        // Ưu tiên 6: Chạy sau khi có User, Store, Product...
        public int Priority => 6;

        public async Task SeedAsync()
        {
            if (await _orderCollection.CountDocumentsAsync(FilterDefinition<Order>.Empty) > 0) return;

            Console.WriteLine("[OrderModule] Đang khởi tạo dữ liệu Đơn hàng...");

            // 1. Lấy danh sách AccountId là "User" (Người mua)
            var accountCollection = _database.GetCollection<BsonDocument>("Accounts");
            var userIds = await accountCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Role", "User"))
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            // 2. Lấy danh sách ProductId (Sản phẩm)
            var productCollection = _database.GetCollection<BsonDocument>("Products");
            var productIds = await productCollection
                .Find(new BsonDocument())
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            if (!userIds.Any() || !productIds.Any())
            {
                Console.WriteLine("[OrderModule] Thiếu dữ liệu User hoặc Product. Hủy tạo Đơn hàng.");
                return;
            }

            // 3. Sinh dữ liệu Đơn hàng
            int totalOrders = Random.Shared.Next(1000, 3000);
            var faker = new Faker<Order>("vi")
                .RuleFor(o => o.AccountId, f => f.PickRandom(userIds))
                .RuleFor(o => o.ProductId, f => f.PickRandom(productIds))
                .RuleFor(o => o.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(o => o.TotalPrice, f => decimal.Parse(f.Commerce.Price(50000, 10000000))) // Tổng tiền giả lập
                .RuleFor(o => o.Status, f => f.PickRandom("Pending", "Processing", "Shipped", "Delivered", "Cancelled"))
                .RuleFor(o => o.OrderDate, f => f.Date.Past(1)); // Đặt hàng trong 1 năm qua

            var fakeOrders = faker.Generate(totalOrders);

            await _orderCollection.InsertManyAsync(fakeOrders);
            Console.WriteLine($"[OrderModule] Đã tạo thành công {totalOrders} Đơn hàng!");
        }
    }
}
