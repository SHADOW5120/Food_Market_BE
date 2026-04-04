using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Modules.OrderModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.OrderModule.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderRepository(MongoDbContext database)
        {
            _orders = database.GetCollection<Order>("Orders");
        }

        public async Task<Order?> GetByIdAsync(string orderId)
        {
            return await _orders.Find(x => x.Id == orderId).FirstOrDefaultAsync();
        }

        public async Task<(List<Order> Orders, long TotalCount)> GetByUserIdAsync(string userId, int page, int pageSize)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.UserId, userId);

            var totalCount = await _orders.CountDocumentsAsync(filter);

            var orders = await _orders.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<List<Order>> GetAllAsync(int page, int pageSize, string? status = null)
        {
            var filter = string.IsNullOrWhiteSpace(status)
                ? Builders<Order>.Filter.Empty
                : Builders<Order>.Filter.Eq(x => x.Status, status);

            return await _orders.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task CreateAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
        }

        public async Task UpdateAsync(Order order)
        {
            await _orders.ReplaceOneAsync(x => x.Id == order.Id, order);
        }

        public async Task<bool> UpdateStatusAsync(string orderId, string newStatus)
        {
            var update = Builders<Order>.Update
                .Set(x => x.Status, newStatus)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _orders.UpdateOneAsync(x => x.Id == orderId, update);

            return result.ModifiedCount > 0;
        }
    }
}
