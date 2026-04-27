namespace Food_Market_BE.Modules.ProductModule.DTOs.Options
{
    public class ProductOptDto
    {
        public string Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public bool IsRequired { get; set; }

        public bool IsMultiple { get; set; }

        public List<ProductOptValueDto> Values { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
