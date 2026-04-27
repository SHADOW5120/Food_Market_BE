using Food_Market_BE.Modules.ProductModule.DTOs.Media;
using Food_Market_BE.Modules.ProductModule.DTOs.Options;

namespace Food_Market_BE.Modules.ProductModule.DTOs.Product
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = default!;

        public List<CreateProductImgRequest>? Images { get; set; }

        public List<CreateProductOptRequest>? Options { get; set; }
    }
}
