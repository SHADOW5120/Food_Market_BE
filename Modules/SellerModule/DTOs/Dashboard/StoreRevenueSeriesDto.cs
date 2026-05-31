namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class StoreRevenueSeriesDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string Label { get; set; }
        public DateTime Time { get; set; }

        public decimal Revenue { get; set; }
    }
}
