using Food_Market_BE.Modules.SellerModule.DTOs.Dashboard;

namespace Food_Market_BE.Modules.SellerModule.Repositories.Interfaces
{
    public interface ISellerRepository
    {
        // ================= SUMMARY =================
        Task<SellerDashboardSummaryDto> GetDashboardSummaryAsync(string sellerId);

        // ================= REVENUE CHARTS =================

        // Tổng doanh thu tất cả store theo thời gian (line/bar)
        Task<List<TimeSeriesStatDto>> GetStoreRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        // Doanh thu từng store theo thời gian (line/bar)
        Task<List<StoreRevenueSeriesDto>> GetRevenueByStoreTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        // Doanh thu tất cả sản phẩm theo thời gian (line/bar)
        Task<List<TimeSeriesStatDto>> GetProductRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        // Doanh thu từng sản phẩm theo thời gian (line/bar)
        Task<List<ProductRevenueSeriesDto>> GetRevenueByProductTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        // Doanh thu đơn hàng theo thời gian (line/bar)
        Task<List<TimeSeriesStatDto>> GetOrderRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day");

        // ================= PIE CHART =================

        // Tỉ lệ doanh thu giữa các store (pie)
        Task<List<StoreRevenuePieDto>> GetStoreRevenuePieStatsAsync(string sellerId);

        // Tỉ lệ doanh thu giữa các sản phẩm (pie)
        Task<List<ProductRevenuePieDto>> GetProductRevenuePieStatsAsync(string sellerId);

        // Tỉ lệ trạng thái đơn hàng (pie)
        Task<List<StatusStatDto>> GetOrderStatusStatsAsync(string sellerId);

        // ================= STORE STATS =================

        // Thống kê doanh thu từng store
        Task<List<StoreStatsDto>> GetStoreStatsAsync(string sellerId);

        // Top store doanh thu cao nhất
        Task<List<TopStoreRevenueDto>> GetTopRevenueStoresAsync(string sellerId, int top);

        // ================= PRODUCT STATS =================

        // Thống kê doanh thu tất cả sản phẩm
        Task<List<ProductStatsDto>> GetAllProductRevenueStatsAsync(string sellerId);

        // Thống kê doanh thu sản phẩm theo store
        Task<List<ProductStatsDto>> GetStoreProductRevenueStatsAsync(string storeId);

        // Top sản phẩm doanh thu cao nhất
        Task<List<TopProductDto>> GetTopRevenueProductsAsync(string sellerId, int top);

        // ================= COUNTS =================

        Task<int> CountCustomersAsync(string sellerId);
        Task<int> CountTotalProductsAsync(string sellerId);
        Task<int> CountTotalStoresAsync(string sellerId);
        Task<int> CountTotalOrdersAsync(string sellerId);
    }
}