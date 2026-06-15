using Bogus;
using Food_Market_BE.Modules.VoucherModule.Models;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Seeder.Interfaces;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.DataSeeder
{
    public class VoucherSeeder : IDataSeeder
    {
        private readonly IMongoCollection<Voucher> _voucherCollection;

        public VoucherSeeder(MongoDbContext database)
        {
            _voucherCollection = database.GetCollection<Voucher>("Vouchers");
        }

        public int Priority => 2;

        public async Task SeedAsync()
        {
            if (await _voucherCollection.CountDocumentsAsync(FilterDefinition<Voucher>.Empty) > 0) return;

            var faker = new Faker<Voucher>("vi")
                .RuleFor(v => v.Code, f => f.Commerce.Ean8()) // Mã giảm giá gồm 8 số
                .RuleFor(v => v.DiscountPercent, f => f.Random.Int(5, 50)) // Giảm từ 5% đến 50%
                .RuleFor(v => v.MaxDiscountAmount, f => f.Random.Int(10000, 200000))
                .RuleFor(v => v.ExpiryDate, f => f.Date.Future(1)) // Hết hạn trong vòng 1 năm tới
                .RuleFor(v => v.DiscountAmount, f => f.Random.Int(50, 1000));

            var fakeVouchers = faker.Generate(30); // Tạo 30 mã giảm giá
            await _voucherCollection.InsertManyAsync(fakeVouchers);
        }
    }
}
