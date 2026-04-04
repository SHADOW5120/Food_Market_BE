namespace Food_Market_BE.Modules.OrderModule.DTOs
{
    public class OrderResponse
    {
        public string OrderId { get; set; } = default!;

        public string Status { get; set; } = default!;

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
