namespace Food_Market_BE.Modules.CartModule.Dtos
{
    public class CartResponse
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
