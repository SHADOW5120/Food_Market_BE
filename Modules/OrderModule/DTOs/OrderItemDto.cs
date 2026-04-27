namespace Food_Market_BE.Modules.OrderModule.DTOs
{
    public class OrderItemDto
    {
        public string ProductId { get; set; } = default!;

        public string ProductName { get; set; } = default!;

        public string? ProductImage { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public List<OrderItemOptDto> Options { get; set; } = new();

        public decimal Subtotal { get; set; }
    }
}
