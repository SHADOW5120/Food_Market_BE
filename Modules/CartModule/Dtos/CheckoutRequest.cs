using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.CartModule.Dtos
{
    public class CheckoutRequest
    {
        [Required]
        public string DeliveryAddress { get; set; } = default!;

        [Required]
        public string PaymentMethod { get; set; } = default!;

        public string? Note { get; set; }
    }
}
