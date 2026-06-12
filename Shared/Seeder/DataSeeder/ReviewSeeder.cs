using Bogus;
using Food_Market_BE.Modules.ReviewModule.Models;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class ReviewSeeder : IDataSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Review> _reviewCollection;

        public ReviewSeeder(IMongoDatabase database)
        {
            _database = database;
            _reviewCollection = database.GetCollection<Review>("Reviews");
        }

        // Ưu tiên 7: Đảm bảo bảng Order đã được gen xong dữ liệu
        public int Priority => 7;

        public async Task SeedAsync()
        {
            if (await _reviewCollection.CountDocumentsAsync(FilterDefinition<Review>.Empty) > 0) return;

            Console.WriteLine("[ReviewModule] Đang phân tích Đơn hàng hoàn tất để tạo Đánh giá...");

            // 1. Quét collection Orders, chỉ lấy những đơn hàng "Delivered"
            var orderCollection = _database.GetCollection<BsonDocument>("Orders");
            var deliveredOrders = await orderCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Status", "Delivered"))
                .Project(b => new
                {
                    OrderId = b["_id"].ToString(),
                    AccountId = b["AccountId"].ToString(),
                    ProductId = b["ProductId"].ToString()
                })
                .ToListAsync();

            if (!deliveredOrders.Any())
            {
                Console.WriteLine("[ReviewModule] Không có đơn hàng nào được giao thành công. Bỏ qua tạo Đánh giá.");
                return;
            }

            // 2. Không phải ai mua xong cũng đánh giá (giả lập tỷ lệ đánh giá là 40%)
            int reviewsToGenerate = (int)(deliveredOrders.Count * 0.4);
            var selectedOrders = new Faker().PickRandom(deliveredOrders, reviewsToGenerate).ToList();

            var reviews = new List<Review>();
            var faker = new Faker("vi");

            foreach (var order in selectedOrders)
            {
                reviews.Add(new Review
                {
                    OrderId = order.OrderId,
                    AccountId = order.AccountId,
                    ProductId = order.ProductId,

                    // Giả lập rating: Khách hàng thường review 4-5 sao, thỉnh thoảng có 1-3 sao
                    Rating = faker.Random.WeightedRandom(
                        new[] { 1, 2, 3, 4, 5 },
                        new[] { 0.05f, 0.05f, 0.1f, 0.3f, 0.5f }
                    ),

                    Comment = faker.Lorem.Sentence(faker.Random.Int(5, 20)), // Bình luận từ 5 - 20 từ
                    CreatedAt = faker.Date.Recent(30)
                });
            }

            await _reviewCollection.InsertManyAsync(reviews);
            Console.WriteLine($"[ReviewModule] Đã tạo thành công {reviews.Count} Đánh giá thực tế!");
        }
    }
}
