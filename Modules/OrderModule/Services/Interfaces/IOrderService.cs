using Food_Market_BE.Modules.OrderModule.DTOs;

namespace Food_Market_BE.Modules.OrderModule.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDetailDto> CreateOrderAsync(CreateOrderRequest request, string userId);
        Task<PagedOrderResponse> GetUserOrdersAsync(string userId, int page, int pageSize);
        Task<OrderDetailDto?> GetOrderDetailAsync(string orderId, string userId, bool isAdmin = false);
        Task<bool> CancelOrderAsync(string orderId, string userId);
        Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus);
    }
}
