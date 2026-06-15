using Bogus;
using Food_Market_BE.Modules.AuthModule.Models;
using Food_Market_BE.Modules.CartModule.Models;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class CartSeeder : IDataSeeder
    {
        private readonly MongoDbContext _database;
        private readonly IMongoCollection<Cart> _cartCollection;

        public CartSeeder(MongoDbContext database)
        {
            _database = database;
            _cartCollection = database.GetCollection<Cart>("Carts");
        }

        public int Priority => 5;

        public async Task SeedAsync()
        {
            if (await _cartCollection.CountDocumentsAsync(FilterDefinition<Cart>.Empty) > 0)
                return;

            Console.WriteLine("[CartSeeder] Fetching Users and Products...");

            var userCollection = _database.GetCollection<BsonDocument>("Users");
            var productCollection = _database.GetCollection<BsonDocument>("Products");

            // -----------------------------
            // 1. GET USERS (safe)
            // -----------------------------
            var userDocs = await userCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Role", UserRole.User))
                .ToListAsync();

            var userIds = userDocs
                .Select(x => x["_id"].ToString())
                .ToList();

            // -----------------------------
            // 2. GET PRODUCTS (safe raw BSON)
            // -----------------------------
            var productDocs = await productCollection
                .Find(new BsonDocument())
                .ToListAsync();

            var products = productDocs.Select(x => new
            {
                Id = x["_id"].ToString(),
                Name = x.GetValue("Name", "").AsString,
                Price = x.GetValue("Price", 0).ToDouble(),

                Images = x.Contains("Images")
                    ? x["Images"].AsBsonArray.Select(i => i.ToString()).ToList()
                    : new List<string>()
            }).ToList();

            // -----------------------------
            // 3. VALIDATION
            // -----------------------------
            if (!userIds.Any() || !products.Any())
            {
                Console.WriteLine("[CartSeeder] Missing Users or Products. Skip seeding.");
                return;
            }

            Console.WriteLine($"[CartSeeder] Users: {userIds.Count}, Products: {products.Count}");

            var faker = new Faker("en");
            var carts = new List<Cart>();

            int totalCarts = Random.Shared.Next(200, 400);

            // -----------------------------
            // 4. GENERATE CARTS
            // -----------------------------
            for (int i = 0; i < totalCarts; i++)
            {
                var userId = faker.PickRandom(userIds);
                var itemCount = faker.Random.Int(1, 5);
                var chosenProducts = faker.PickRandom(products, itemCount).ToList();

                var items = chosenProducts.Select(p =>
                {
                    var qty = faker.Random.Int(1, 5);

                    return new CartItem
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        ProductId = p.Id,
                        ProductName = p.Name,
                        ProductImage = p.Images.FirstOrDefault(),
                        Price = 100,
                        Quantity = qty,
                        Options = new List<CartItemOpt>(),
                        Subtotal = 200,
                        CreatedAt = faker.Date.Recent(30)
                    };
                }).ToList();

                carts.Add(new Cart
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = userId,
                    Items = items,
                    TotalPrice = items.Sum(x => x.Subtotal),
                    CreatedAt = faker.Date.Recent(30),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _cartCollection.InsertManyAsync(carts);

            Console.WriteLine($"[CartSeeder] Created {carts.Count} carts successfully.");
        }
    }
}