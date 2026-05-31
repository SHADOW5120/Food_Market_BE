namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class StoreRevenuePieDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }

        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
    }
}
