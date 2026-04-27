namespace Food_Market_BE.Modules.ProductModule.DTOs.Options
{
    public class ProductOptValueDto
    {
        public string Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public decimal PriceModifier { get; set; }
    }
}
