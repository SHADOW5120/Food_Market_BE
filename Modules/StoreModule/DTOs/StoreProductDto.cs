namespace Food_Market_BE.Modules.StoreModule.DTOs
{
    public class StoreProductDto
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
