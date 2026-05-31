namespace Food_Market_BE.Modules.SellerModule.DTOs.Dashboard
{
    public class StatusStatDto
    {
        public string Status { get; set; } = string.Empty; // Pending, Shipped...
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
