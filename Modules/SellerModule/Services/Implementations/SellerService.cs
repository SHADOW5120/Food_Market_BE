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

        // ================= SUMMARY =================

        public async Task<SellerDashboardSummaryDto> GetDashboardSummaryAsync(string sellerId)
        {
            return await _sellerRepository.GetDashboardSummaryAsync(sellerId);
        }

        // ================= REVENUE CHARTS =================

        public async Task<List<TimeSeriesStatDto>> GetStoreRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            return await _sellerRepository.GetStoreRevenueTimeSeriesAsync(sellerId, from, to, groupBy);
        }

        public async Task<List<StoreRevenueSeriesDto>> GetRevenueByStoreTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            return await _sellerRepository.GetRevenueByStoreTimeSeriesAsync(sellerId, from, to, groupBy);
        }

        public async Task<List<TimeSeriesStatDto>> GetProductRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            return await _sellerRepository.GetProductRevenueTimeSeriesAsync(sellerId, from, to, groupBy);
        }

        public async Task<List<ProductRevenueSeriesDto>> GetRevenueByProductTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            return await _sellerRepository.GetRevenueByProductTimeSeriesAsync(sellerId, from, to, groupBy);
        }

        public async Task<List<TimeSeriesStatDto>> GetOrderRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            return await _sellerRepository.GetOrderRevenueTimeSeriesAsync(sellerId, from, to, groupBy);
        }

        // ================= PIE CHART =================

        public async Task<List<StoreRevenuePieDto>> GetStoreRevenuePieStatsAsync(string sellerId)
        {
            return await _sellerRepository.GetStoreRevenuePieStatsAsync(sellerId);
        }

        public async Task<List<ProductRevenuePieDto>> GetProductRevenuePieStatsAsync(string sellerId)
        {
            return await _sellerRepository.GetProductRevenuePieStatsAsync(sellerId);
        }

        public async Task<List<StatusStatDto>> GetOrderStatusStatsAsync(string sellerId)
        {
            return await _sellerRepository.GetOrderStatusStatsAsync(sellerId);
        }

        // ================= STORE STATS =================

        public async Task<List<StoreStatsDto>> GetStoreStatsAsync(string sellerId)
        {
            return await _sellerRepository.GetStoreStatsAsync(sellerId);
        }

        public async Task<List<TopStoreRevenueDto>> GetTopRevenueStoresAsync(string sellerId, int top)
        {
            return await _sellerRepository.GetTopRevenueStoresAsync(sellerId, top);
        }

        // ================= PRODUCT STATS =================

        public async Task<List<ProductStatsDto>> GetAllProductRevenueStatsAsync(string sellerId)
        {
            return await _sellerRepository.GetAllProductRevenueStatsAsync(sellerId);
        }

        public async Task<List<ProductStatsDto>> GetStoreProductRevenueStatsAsync(string storeId)
        {
            return await _sellerRepository.GetStoreProductRevenueStatsAsync(storeId);
        }

        public async Task<List<TopProductDto>> GetTopRevenueProductsAsync(string sellerId, int top)
        {
            return await _sellerRepository.GetTopRevenueProductsAsync(sellerId, top);
        }

        // ================= COUNTS =================

        public async Task<int> CountCustomersAsync(string sellerId)
        {
            return await _sellerRepository.CountCustomersAsync(sellerId);
        }

        public async Task<int> CountTotalProductsAsync(string sellerId)
        {
            return await _sellerRepository.CountTotalProductsAsync(sellerId);
        }

        public async Task<int> CountTotalStoresAsync(string sellerId)
        {
            return await _sellerRepository.CountTotalStoresAsync(sellerId);
        }

        public async Task<int> CountTotalOrdersAsync(string sellerId)
        {
            return await _sellerRepository.CountTotalOrdersAsync(sellerId);
        }

        // ================= NOTIFICATIONS =================

        public async Task<List<SellerNotificationDto>> GetNotificationsAsync(string userId)
        {
            return [];
        }

        public async Task<bool> MarkNotificationAsReadAsync(string userId, string id)
        {
            return true;
        }
    }
}