using Food_Market_BE.Modules.CategoryModule.Models;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class CategorySeeder : IDataSeeder
    {
        private readonly IMongoCollection<Category> _categoryCollection;

        public CategorySeeder(MongoDbContext database)
        {
            _categoryCollection = database.GetCollection<Category>("Categories");
        }

        public int Priority => 1;

        public async Task SeedAsync()
        {
            if (await _categoryCollection.CountDocumentsAsync(FilterDefinition<Category>.Empty) > 0) return;

            var predefinedCategories = new List<string>
            {
                "Điện thoại & Phụ kiện", "Máy tính & Laptop", "Thiết bị điện tử",
                "Thời trang Nam", "Thời trang Nữ", "Đồ gia dụng",
                "Mẹ & Bé", "Sức khỏe & Sắc đẹp"
            };

            var categoriesToInsert = predefinedCategories.Select(name => new Category
            {
                Name = name,
                // Map các field khác của bạn ở đây nếu có
                // Ví dụ: Description = $"Mô tả cho {name}"
            }).ToList();

            await _categoryCollection.InsertManyAsync(categoriesToInsert);
        }
    }
}
