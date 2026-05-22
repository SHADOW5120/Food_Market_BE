namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class SellerDashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
    }
}