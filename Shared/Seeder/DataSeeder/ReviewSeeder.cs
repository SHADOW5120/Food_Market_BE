using Bogus;
using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Modules.ReviewModule.Models;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class ReviewSeeder : IDataSeeder
    {
        private readonly MongoDbContext _database;
        private readonly IMongoCollection<Review> _reviewCollection;

        public ReviewSeeder(MongoDbContext database)
        {
            _database = database;
            _reviewCollection = database.GetCollection<Review>("Reviews");
        }

        public int Priority => 7;

        public async Task SeedAsync()
        {
            // Skip if already seeded
            if (await _reviewCollection.CountDocumentsAsync(FilterDefinition<Review>.Empty) > 0)
                return;

            Console.WriteLine("[ReviewSeeder] Generating reviews from completed orders...");

            var orderCollection = _database.GetCollection<BsonDocument>("Orders");

            // -----------------------------
            // SAFE QUERY COMPLETED ORDERS
            // -----------------------------
            var completedOrders = await orderCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Status", OrderStatus.Completed.ToString()))
                .ToListAsync();

            if (!completedOrders.Any())
            {
                Console.WriteLine("[ReviewSeeder] No completed orders found. Skipping.");
                return;
            }

            var faker = new Faker("en");
            var reviews = new List<Review>();

            // -----------------------------
            // PROCESS ORDERS SAFELY
            // -----------------------------
            foreach (var order in completedOrders)
            {
                // Safe get userId
                var userId = order.GetValue("UserId", "").ToString();
                if (string.IsNullOrWhiteSpace(userId))
                    continue;

                // Safe get items array
                if (!order.Contains("Items"))
                    continue;

                var items = order["Items"].AsBsonArray;

                if (items == null || items.Count == 0)
                    continue;

                // Pick random item safely
                var randomItem = faker.PickRandom(items);

                if (randomItem == null || !randomItem.IsBsonDocument)
                    continue;

                var itemDoc = randomItem.AsBsonDocument;

                var productId = itemDoc.GetValue("ProductId", "").ToString();
                if (string.IsNullOrWhiteSpace(productId))
                    continue;

                // -----------------------------
                // CREATE REVIEW
                // -----------------------------
                reviews.Add(new Review
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = userId,
                    ProductId = productId,

                    Rating = faker.PickRandom(new[] { 1, 2, 3, 4, 5 }),

                    Comment = faker.Random.Bool(0.8f)
                        ? faker.Lorem.Sentence(faker.Random.Int(5, 15))
                        : null,

                    Images = new List<string>(),

                    CreatedAt = faker.Date.Recent(60)
                });
            }

            // -----------------------------
            // VALIDATION
            // -----------------------------
            if (!reviews.Any())
            {
                Console.WriteLine("[ReviewSeeder] No valid reviews generated.");
                return;
            }

            // -----------------------------
            // INSERT
            // -----------------------------
            await _reviewCollection.InsertManyAsync(reviews);

            Console.WriteLine($"[ReviewSeeder] Created {reviews.Count} reviews successfully.");
        }
    }
}