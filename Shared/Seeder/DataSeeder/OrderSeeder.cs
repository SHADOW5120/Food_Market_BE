using Bogus;
using Food_Market_BE.Modules.AuthModule.Models;
using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class OrderSeeder : IDataSeeder
    {
        private readonly MongoDbContext _database;
        private readonly IMongoCollection<Order> _orderCollection;

        public OrderSeeder(MongoDbContext database)
        {
            _database = database;
            _orderCollection = database.GetCollection<Order>("Orders");
        }

        public int Priority => 6;

        public async Task SeedAsync()
        {
            // Skip if already seeded
            if (await _orderCollection.CountDocumentsAsync(FilterDefinition<Order>.Empty) > 0)
                return;

            Console.WriteLine("[OrderSeeder] Generating fake orders...");

            var userCollection = _database.GetCollection<BsonDocument>("Users");
            var productCollection = _database.GetCollection<BsonDocument>("Products");

            // -----------------------------
            // USERS (safe load)
            // -----------------------------
            var userDocs = await userCollection
                .Find(Builders<BsonDocument>.Filter.Eq("Role", UserRole.User))
                .ToListAsync();

            var userIds = userDocs
                .Select(x => x["_id"].ToString())
                .ToList();

            // -----------------------------
            // PRODUCTS (safe parse)
            // -----------------------------
            var productDocs = await productCollection
                .Find(new BsonDocument())
                .ToListAsync();

            var products = productDocs.Select(x => new
            {
                Id = x["_id"].ToString(),
                Name = x.GetValue("Name", "").AsString,
                Price = Convert.ToDecimal(x.GetValue("Price", 0).ToDouble()),

                Images = x.Contains("Images")
                    ? x["Images"].AsBsonArray.Select(i => i.ToString()).ToList()
                    : new List<string>()
            }).ToList();

            // -----------------------------
            // VALIDATION
            // -----------------------------
            if (!userIds.Any() || !products.Any())
            {
                Console.WriteLine("[OrderSeeder] Missing users or products. Skipping.");
                return;
            }

            Console.WriteLine($"[OrderSeeder] Users: {userIds.Count}, Products: {products.Count}");

            var faker = new Faker("en");
            var orders = new List<Order>();

            var paymentMethods = new[] { "Cash on Delivery", "Visa", "Mastercard", "MOMO" };

            var statusOptions = new[]
            {
                OrderStatus.Pending,
                OrderStatus.Confirmed,
                OrderStatus.Preparing,
                OrderStatus.Delivering,
                OrderStatus.Completed,
                OrderStatus.Cancelled
            };

            int totalOrders = Random.Shared.Next(300, 800);

            for (int i = 0; i < totalOrders; i++)
            {
                var userId = faker.PickRandom(userIds);

                var itemCount = faker.Random.Int(1, 4);
                var chosenProducts = faker.PickRandom(products, itemCount).ToList();

                var items = chosenProducts.Select(p =>
                {
                    var qty = faker.Random.Int(1, 5);
                    var unitPrice = p.Price;

                    return new OrderItem
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        ProductId = p.Id,
                        ProductName = p.Name,
                        ProductImage = p.Images.FirstOrDefault(),
                        UnitPrice = unitPrice,
                        Quantity = qty,
                        Options = new List<OrderItemOpt>(),
                        Subtotal = unitPrice * qty
                    };
                }).ToList();

                var subtotal = items.Sum(x => x.Subtotal);

                // Safe discount calculation
                var discountAmount = faker.Random.Bool(0.3f)
                    ? Math.Round(subtotal * (decimal)faker.Random.Double(0.05, 0.25), 2)
                    : 0;

                var shippingFee = faker.Random.Int(15000, 50000);

                var totalPrice = subtotal - discountAmount + shippingFee;

                var status = faker.PickRandom(statusOptions);
                var createdAt = faker.Date.Past(1);

                orders.Add(new Order
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = userId,
                    Items = items,
                    Subtotal = subtotal,
                    DiscountAmount = discountAmount,
                    ShippingFee = shippingFee,
                    TotalPrice = totalPrice,
                    Status = status,

                    DeliveryAddress = new DeliveryAddress
                    {
                        ReceiverName = faker.Name.FullName(),
                        PhoneNumber = faker.Phone.PhoneNumber("0#########"),
                        AddressLine = faker.Address.FullAddress()
                    },

                    PaymentMethod = faker.PickRandom(paymentMethods),
                    VoucherCode = faker.Random.Bool(0.2f) ? faker.Random.AlphaNumeric(8) : null,
                    Note = faker.Random.Bool(0.3f) ? faker.Lorem.Sentence(8) : null,

                    CreatedAt = createdAt,
                    UpdatedAt = createdAt.AddHours(faker.Random.Int(1, 72)),
                    CancelledAt = status == OrderStatus.Cancelled
                        ? createdAt.AddDays(faker.Random.Int(1, 14))
                        : null
                });
            }

            await _orderCollection.InsertManyAsync(orders);

            Console.WriteLine($"[OrderSeeder] Created {orders.Count} orders successfully.");
        }
    }
}