using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.OrderModule.DTOs
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        public OrderStatus NewStatus { get; set; } = default!;
    }
}
