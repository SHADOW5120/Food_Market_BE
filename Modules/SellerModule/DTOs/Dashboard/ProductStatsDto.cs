namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class ProductStatsDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public string StoreId { get; set; }
        public string StoreName { get; set; }

        public int TotalSoldQuantity { get; set; }
        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
