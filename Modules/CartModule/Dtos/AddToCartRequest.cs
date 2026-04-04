using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.CartModule.Dtos
{
    public class AddToCartRequest
    {
        [Required]
        public string ProductId { get; set; } = default!;

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
