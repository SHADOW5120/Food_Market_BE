using Food_Market_BE.Modules.OrderModule.Models;

namespace Food_Market_BE.Modules.OrderModule.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(string orderId);
        Task<(List<Order> Orders, long TotalCount)> GetByUserIdAsync(string userId, int page, int pageSize);
        Task<List<Order>> GetAllAsync(int page, int pageSize, OrderStatus? status = null);
        Task CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task<bool> UpdateStatusAsync(string orderId, OrderStatus newStatus);
    }
}
