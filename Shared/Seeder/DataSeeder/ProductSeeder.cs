using Bogus;
using Food_Market_BE.Modules.ProductModule.Models.Media;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class ProductSeeder : IDataSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _productCollection;

        public ProductSeeder(IMongoDatabase database)
        {
            _database = database;
            _productCollection = database.GetCollection<Product>("Products");
        }

        // Ưu tiên 4: Chạy sau khi đã có Danh mục (Nhóm 1) và Cửa hàng (Nhóm 3)
        public int Priority => 4;

        public async Task SeedAsync()
        {
            if (await _productCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty) > 0)
            {
                return;
            }

            Console.WriteLine("[ProductModule] Đang lấy thông tin Danh mục và Cửa hàng để tạo Sản phẩm...");

            // 1. TRUY VẤN XUYÊN MODULE: Lấy danh sách CategoryId và StoreId
            var categoryCollection = _database.GetCollection<BsonDocument>("Categories");
            var categoryIds = await categoryCollection
                .Find(new BsonDocument())
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            var storeCollection = _database.GetCollection<BsonDocument>("Stores");
            var storeIds = await storeCollection
                .Find(new BsonDocument())
                .Project(b => b["_id"].ToString())
                .ToListAsync();

            // Kiểm tra an toàn
            if (!categoryIds.Any() || !storeIds.Any())
            {
                Console.WriteLine("[ProductModule] LỖI: Thiếu dữ liệu Danh mục hoặc Cửa hàng. Hủy tạo Sản phẩm.");
                return;
            }

            // 2. SINH DỮ LIỆU: Tạo khoảng 1000 - 2000 sản phẩm ngẫu nhiên
            int totalProducts = Random.Shared.Next(1000, 2001);
            var faker = new Faker<Product>("vi")
                .RuleFor(p => p.CategoryId, f => f.PickRandom(categoryIds)) // Chọn ngẫu nhiên 1 danh mục
                .RuleFor(p => p.StoreId, f => f.PickRandom(storeIds))       // Chọn ngẫu nhiên 1 cửa hàng bán
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(50000, 5000000))) // Giá từ 50k đến 5 triệu
                .RuleFor(p => p.Popularity, f => f.Random.Int(10, 1000))
                .RuleFor(p => p.Images, f => new List<ProductImg>
                {
                    f.Image.PicsumUrl(),
                    f.Image.PicsumUrl()
                }) // Random 2 link ảnh minh họa
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(1));

            var fakeProducts = faker.Generate(totalProducts);

            // 3. BULK INSERT VÀO DATABASE
            await _productCollection.InsertManyAsync(fakeProducts);
            Console.WriteLine($"[ProductModule] Đã tạo thành công {totalProducts} Sản phẩm!");
        }
    }
}
