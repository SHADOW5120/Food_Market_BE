namespace Food_Market_BE.Modules.CartModule.Dtos
{
    public class CartItemOptDto
    {
        public string OptionId { get; set; } = default!;
        public string ValueId { get; set; } = default!;
        public string OptionName { get; set; } = default!;
        public string ValueName { get; set; } = default!;
        public decimal PriceModifier { get; set; }
    }
}
