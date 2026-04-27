namespace Food_Market_BE.Modules.ProductModule.DTOs.Options
{
    public class CreateProductOptValueRequest
    {
        public string Name { get; set; } = default!;

        public decimal PriceModifier { get; set; } = 0;
    }
}
