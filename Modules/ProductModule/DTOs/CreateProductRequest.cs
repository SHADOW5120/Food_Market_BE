namespace Food_Market_BE.Modules.ProductModule.DTOs
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = default!;
        public string? ImageUrl { get; set; }
    }
}
