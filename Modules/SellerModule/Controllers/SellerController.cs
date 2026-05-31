using Food_Market_BE.Modules.SellerModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.SellerModule.Controllers
{
    [ApiController]
    [Route("api/seller")]
    [Authorize(Roles = "Seller")]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ================= SUMMARY =================

        [HttpGet("dashboard/summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _sellerService.GetDashboardSummaryAsync(UserId);
            return Ok(result);
        }

        // ================= REVENUE CHARTS =================

        [HttpGet("dashboard/charts/store-revenue")]
        public async Task<IActionResult> GetStoreRevenueTimeSeries([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string groupBy = "day")
        {
            var result = await _sellerService.GetStoreRevenueTimeSeriesAsync(UserId, from, to, groupBy);
            return Ok(result);
        }

        [HttpGet("dashboard/charts/store-revenue-detail")]
        public async Task<IActionResult> GetRevenueByStoreTimeSeries([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string groupBy = "day")
        {
            var result = await _sellerService.GetRevenueByStoreTimeSeriesAsync(UserId, from, to, groupBy);
            return Ok(result);
        }

        [HttpGet("dashboard/charts/product-revenue")]
        public async Task<IActionResult> GetProductRevenueTimeSeries([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string groupBy = "day")
        {
            var result = await _sellerService.GetProductRevenueTimeSeriesAsync(UserId, from, to, groupBy);
            return Ok(result);
        }

        [HttpGet("dashboard/charts/product-revenue-detail")]
        public async Task<IActionResult> GetRevenueByProductTimeSeries([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string groupBy = "day")
        {
            var result = await _sellerService.GetRevenueByProductTimeSeriesAsync(UserId, from, to, groupBy);
            return Ok(result);
        }

        [HttpGet("dashboard/charts/order-revenue")]
        public async Task<IActionResult> GetOrderRevenueTimeSeries([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string groupBy = "day")
        {
            var result = await _sellerService.GetOrderRevenueTimeSeriesAsync(UserId, from, to, groupBy);
            return Ok(result);
        }

        // ================= PIE CHART =================

        [HttpGet("dashboard/pie/store-revenue")]
        public async Task<IActionResult> GetStoreRevenuePieStats()
        {
            var result = await _sellerService.GetStoreRevenuePieStatsAsync(UserId);
            return Ok(result);
        }

        [HttpGet("dashboard/pie/product-revenue")]
        public async Task<IActionResult> GetProductRevenuePieStats()
        {
            var result = await _sellerService.GetProductRevenuePieStatsAsync(UserId);
            return Ok(result);
        }

        [HttpGet("dashboard/pie/order-status")]
        public async Task<IActionResult> GetOrderStatusStats()
        {
            var result = await _sellerService.GetOrderStatusStatsAsync(UserId);
            return Ok(result);
        }

        // ================= STORE STATS =================

        [HttpGet("stores/stats")]
        public async Task<IActionResult> GetStoreStats()
        {
            var result = await _sellerService.GetStoreStatsAsync(UserId);
            return Ok(result);
        }

        [HttpGet("stores/top-revenue")]
        public async Task<IActionResult> GetTopRevenueStores([FromQuery] int top = 5)
        {
            var result = await _sellerService.GetTopRevenueStoresAsync(UserId, top);
            return Ok(result);
        }

        // ================= PRODUCT STATS =================

        [HttpGet("products/stats")]
        public async Task<IActionResult> GetAllProductRevenueStats()
        {
            var result = await _sellerService.GetAllProductRevenueStatsAsync(UserId);
            return Ok(result);
        }

        [HttpGet("stores/{storeId}/products/stats")]
        public async Task<IActionResult> GetStoreProductRevenueStats(string storeId)
        {
            var result = await _sellerService.GetStoreProductRevenueStatsAsync(storeId);
            return Ok(result);
        }

        [HttpGet("products/top-revenue")]
        public async Task<IActionResult> GetTopRevenueProducts([FromQuery] int top = 5)
        {
            var result = await _sellerService.GetTopRevenueProductsAsync(UserId, top);
            return Ok(result);
        }

        // ================= COUNTS =================

        [HttpGet("counts/customers")]
        public async Task<IActionResult> CountCustomers()
        {
            var result = await _sellerService.CountCustomersAsync(UserId);
            return Ok(result);
        }

        [HttpGet("counts/products")]
        public async Task<IActionResult> CountTotalProducts()
        {
            var result = await _sellerService.CountTotalProductsAsync(UserId);
            return Ok(result);
        }

        [HttpGet("counts/stores")]
        public async Task<IActionResult> CountTotalStores()
        {
            var result = await _sellerService.CountTotalStoresAsync(UserId);
            return Ok(result);
        }

        [HttpGet("counts/orders")]
        public async Task<IActionResult> CountTotalOrders()
        {
            var result = await _sellerService.CountTotalOrdersAsync(UserId);
            return Ok(result);
        }

        // ================= NOTIFICATIONS =================

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var result = await _sellerService.GetNotificationsAsync(UserId);
            return Ok(result);
        }

        [HttpPut("notifications/{id}/read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            var result = await _sellerService.MarkNotificationAsReadAsync(UserId, id);
            return Ok(result);
        }
    }
}