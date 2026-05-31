namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class TopStoreRevenueDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }

        public decimal TotalRevenue { get; set; }

        public int TotalOrders { get; set; }

        public int TotalProducts { get; set; }
    }
}
