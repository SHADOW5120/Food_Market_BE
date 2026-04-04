using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.CartModule.Dtos
{
    public class UpdateCartItemRequest
    {
        [Range(0, 100)]
        public int Quantity { get; set; }
    }
}
