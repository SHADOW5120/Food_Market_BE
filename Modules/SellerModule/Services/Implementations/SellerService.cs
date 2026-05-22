using Food_Market_BE.Modules.SellerModule.DTOs.Dashboard;
using Food_Market_BE.Modules.SellerModule.DTOs.SellerNotifications;
using Food_Market_BE.Modules.SellerModule.Repositories.Interfaces;
using Food_Market_BE.Modules.SellerModule.Services.Interfaces;

namespace Food_Market_BE.Modules.SellerModule.Services.Implementations
{
    public class SellerService : ISellerService
    {
        private readonly ISellerRepository _sellerRepository;

        public SellerService(ISellerRepository sellerRepository)
        {
            _sellerRepository = sellerRepository;
        }

        // ================= DASHBOARD =================

        public async Task<SellerDashboardStatsDto> GetDashboardStatsAsync(string sellerId)
        {
            return new SellerDashboardStatsDto
            {
                TotalProducts = await _sellerRepository.CountTotalProductsAsync(sellerId),
                TotalOrders = await _sellerRepository.CountTotalOrdersAsync(sellerId),
                TotalRevenue = await _sellerRepository.GetTotalRevenueAsync(sellerId),
                PendingOrders = await _sellerRepository.CountPendingOrdersAsync(sellerId)
            };
        }

        public async Task<List<SellerAnalyticsDto>> GetAnalyticsAsync(
            string sellerId,
            string period)
        {
            return new List<SellerAnalyticsDto>();
        }

        // ================= NOTIFICATIONS =================

        public async Task<List<SellerNotificationDto>> GetNotificationsAsync(
            string sellerId)
        {
            return [];
        }

        public async Task<bool> MarkNotificationAsReadAsync(
            string sellerId,
            string notificationId)
        {
            return true;
        }
    }
}