namespace Food_Market_BE.Modules.OrderModule.DTOs
{
    public class OrderDetailDto
    {
        public string OrderId { get; set; } = default!;

        public string UserId { get; set; } = default!;

        public string CartId { get; set; } = default!;

        public string Status { get; set; } = default!;

        public string DeliveryAddress { get; set; } = default!;

        public string PaymentMethod { get; set; } = default!;

        public string? VoucherCode { get; set; }

        public decimal Subtotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
