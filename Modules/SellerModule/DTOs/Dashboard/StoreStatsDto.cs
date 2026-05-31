namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class StoreStatsDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }

        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }

        public int TotalCustomers { get; set; }
    }
}
