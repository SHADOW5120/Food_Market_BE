namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class SellerDashboardSummaryDto
    {
        public int TotalCustomers { get; set; }

        public int TotalOrders { get; set; }

        public int TotalProducts { get; set; }

        public int TotalStores { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}