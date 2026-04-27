using Food_Market_BE.Modules.CategoryModule.DTOs;
using Food_Market_BE.Modules.ProductModule.DTOs.Media;
using Food_Market_BE.Modules.ProductModule.DTOs.Options;

namespace Food_Market_BE.Modules.ProductModule.DTOs.Product
{
    public class ProductDetailDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public List<ProductImgDto> Images { get; set; } = new();

        public List<ProductOptDto> Options { get; set; } = new();

        public CategoryDto Category { get; set; } = default!;

        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
