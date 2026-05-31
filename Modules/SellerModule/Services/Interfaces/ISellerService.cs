using Food_Market_BE.Modules.SellerModule.DTOs.Dashboard;
using Food_Market_BE.Modules.SellerModule.DTOs.SellerNotifications;

namespace Food_Market_BE.Modules.SellerModule.Services.Interfaces
{
    public interface ISellerService
    {
        // ================= SUMMARY =================

        Task<SellerDashboardSummaryDto> GetDashboardSummaryAsync(string sellerId);

        // ================= REVENUE CHARTS =================

        Task<List<TimeSeriesStatDto>> GetStoreRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        Task<List<StoreRevenueSeriesDto>> GetRevenueByStoreTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        Task<List<TimeSeriesStatDto>> GetProductRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        Task<List<ProductRevenueSeriesDto>> GetRevenueByProductTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        Task<List<TimeSeriesStatDto>> GetOrderRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        // ================= PIE CHART =================

        Task<List<StoreRevenuePieDto>> GetStoreRevenuePieStatsAsync(string sellerId);

        Task<List<ProductRevenuePieDto>> GetProductRevenuePieStatsAsync(string sellerId);

        Task<List<StatusStatDto>> GetOrderStatusStatsAsync(string sellerId);

        // ================= STORE STATS =================

        Task<List<StoreStatsDto>> GetStoreStatsAsync(string sellerId);

        Task<List<TopStoreRevenueDto>> GetTopRevenueStoresAsync(string sellerId, int top);

        // ================= PRODUCT STATS =================

        Task<List<ProductStatsDto>> GetAllProductRevenueStatsAsync(string sellerId);

        Task<List<ProductStatsDto>> GetStoreProductRevenueStatsAsync(string storeId);

        Task<List<TopProductDto>> GetTopRevenueProductsAsync(string sellerId, int top);

        // ================= COUNTS =================

        Task<int> CountCustomersAsync(string sellerId);

        Task<int> CountTotalProductsAsync(string sellerId);

        Task<int> CountTotalStoresAsync(string sellerId);

        Task<int> CountTotalOrdersAsync(string sellerId);

        // ================= NOTIFICATIONS =================

        Task<List<SellerNotificationDto>> GetNotificationsAsync(string userId);

        Task<bool> MarkNotificationAsReadAsync(string userId, string id);
    }
}