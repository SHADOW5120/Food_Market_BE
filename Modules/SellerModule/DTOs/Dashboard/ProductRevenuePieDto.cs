namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class ProductRevenuePieDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
    }
}
