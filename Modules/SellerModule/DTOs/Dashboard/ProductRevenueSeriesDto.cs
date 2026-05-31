namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class ProductRevenueSeriesDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Label { get; set; }
        public DateTime Time { get; set; }

        public decimal Revenue { get; set; }
    }
}
