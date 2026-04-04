using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.OrderModule.DTOs
{
    public class CreateOrderRequest
    {
        [Required]
        public string CartId { get; set; } = default!;

        [Required]
        public string DeliveryAddress { get; set; } = default!;

        [Required]
        public string PaymentMethod { get; set; } = default!;

        public string? VoucherCode { get; set; }
    }
}
