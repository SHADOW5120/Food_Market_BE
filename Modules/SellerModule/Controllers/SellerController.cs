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

        // ---------------- DASHBOARD ----------------
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _sellerService.GetDashboardStatsAsync(UserId);
            return Ok(result);
        }

        [HttpGet("dashboard/analytics")]
        public async Task<IActionResult> GetAnalytics([FromQuery] string period = "month")
        {
            var result = await _sellerService.GetAnalyticsAsync(UserId, period);
            return Ok(result);
        }

        // ---------------- NOTIFICATIONS ----------------
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