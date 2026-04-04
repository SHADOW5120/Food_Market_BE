using Food_Market_BE.Modules.VoucherModule.Models;
using Food_Market_BE.Modules.VoucherModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.VoucherModule.Repositories.Implementations
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly IMongoCollection<Voucher> _voucherCollection;

        public VoucherRepository(MongoDbContext database)
        {
            _voucherCollection = database.GetCollection<Voucher>("Vouchers");
        }

        public async Task<List<Voucher>> GetAllActiveVouchersAsync()
        {
            var now = DateTime.UtcNow;

            return await _voucherCollection
                .Find(v => !v.IsDeleted && v.ExpiryDate >= now)
                .SortBy(v => v.ExpiryDate)
                .ToListAsync();
        }

        public async Task<Voucher?> GetVoucherByIdAsync(string voucherId)
        {
            return await _voucherCollection
                .Find(v => v.Id == voucherId && !v.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Voucher?> GetVoucherByCodeAsync(string code)
        {
            return await _voucherCollection
                .Find(v => v.Code.ToUpper() == code.ToUpper() && !v.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task CreateVoucherAsync(Voucher voucher)
        {
            await _voucherCollection.InsertOneAsync(voucher);
        }

        public async Task UpdateVoucherAsync(Voucher voucher)
        {
            await _voucherCollection.ReplaceOneAsync(v => v.Id == voucher.Id, voucher);
        }

        public async Task DeleteVoucherAsync(string voucherId)
        {
            var update = Builders<Voucher>.Update
                .Set(v => v.IsDeleted, true)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            await _voucherCollection.UpdateOneAsync(v => v.Id == voucherId, update);
        }
    }
}
