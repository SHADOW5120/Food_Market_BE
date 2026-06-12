using Bogus;
using Food_Market_BE.Modules.StoreModule.Models;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class StoreSeeder : IDataSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Store> _storeCollection;

        public StoreSeeder(IMongoDatabase database)
        {
            _database = database;
            _storeCollection = database.GetCollection<Store>("Stores");
        }

        // Ưu tiên 3: Phải chờ Nhóm 2 (Seller) chạy xong để lấy ID
        public int Priority => 3;

        public async Task SeedAsync()
        {
            // Kiểm tra an toàn: Nếu đã có dữ liệu thì thoát luôn để không tạo rác
            if (await _storeCollection.CountDocumentsAsync(FilterDefinition<Store>.Empty) > 0)
            {
                return;
            }

            Console.WriteLine("[StoreModule] Đang tiến hành lấy ID Người bán và tạo Cửa hàng...");

            // 1. TRUY VẤN XUYÊN MODULE: Lấy danh sách ID từ collection Sellers
            // Chúng ta dùng BsonDocument để không bị phụ thuộc code vào SellerModule
            var sellerCollection = _database.GetCollection<BsonDocument>("Sellers");

            var sellerIds = await sellerCollection
                .Find(new BsonDocument())
                .Project(b => b["_id"].ToString()) // Lấy ID của Seller
                .ToListAsync();

            // Nếu DB chưa có Seller nào, ta buộc phải dừng lại để tránh lỗi rỗng ID
            if (!sellerIds.Any())
            {
                Console.WriteLine("[StoreModule] LỖI: Không có Người bán nào. Hủy tạo Cửa hàng.");
                return;
            }

            // 2. SINH DỮ LIỆU: Đảm bảo 1 Seller có đúng 1 Cửa hàng
            var stores = new List<Store>();
            var faker = new Faker("vi");

            foreach (var sellerId in sellerIds)
            {
                stores.Add(new Store
                {
                    SellerId = sellerId, // Gắn chặt Store này vào Seller tương ứng

                    // Tạo tên cửa hàng xịn xò ghép từ tên công ty giả và các hậu tố
                    StoreName = faker.Company.CompanyName() + " " + faker.PickRandom("Shop", "Store", "Official", "Boutique"),

                    Description = faker.Lorem.Paragraph(),
                    Address = faker.Address.FullAddress(),

                    // Điểm đánh giá random từ 3.5 đến 5.0 (làm tròn 1 chữ số thập phân)
                    Rating = Math.Round(faker.Random.Decimal(3.5m, 5.0m), 1),

                    IsActive = true
                });
            }

            // 3. LƯU VÀO DB BẰNG BULK INSERT (Tối ưu tốc độ)
            await _storeCollection.InsertManyAsync(stores);
            Console.WriteLine($"[StoreModule] Đã tạo thành công {stores.Count} Cửa hàng!");
        }
    }
}
