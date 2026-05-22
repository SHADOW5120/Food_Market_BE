using Food_Market_BE.Modules.SellerModule.DTOs.Dashboard;
using Food_Market_BE.Modules.SellerModule.DTOs.SellerNotifications;

namespace Food_Market_BE.Modules.SellerModule.Services.Interfaces
{
    public interface ISellerService
    {
        Task<SellerDashboardStatsDto> GetDashboardStatsAsync(string userId);
        Task<List<SellerAnalyticsDto>> GetAnalyticsAsync(string userId, string period);

        Task<List<SellerNotificationDto>> GetNotificationsAsync(string userId);
        Task<bool> MarkNotificationAsReadAsync(string userId, string id);
    }
}