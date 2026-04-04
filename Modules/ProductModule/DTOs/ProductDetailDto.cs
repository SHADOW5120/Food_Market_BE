namespace Food_Market_BE.Modules.ProductModule.DTOs
{
    public class ProductDetailDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public CategoryDto Category { get; set; } = default!;

        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
