using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.VoucherModule.DTOs
{
    public class ApplyVoucherRequest
    {
        [Required]
        public string UserId { get; set; } = default!;

        [Required]
        public string VoucherCode { get; set; } = default!;

        [Range(0, double.MaxValue)]
        public decimal CartTotal { get; set; }
    }
}
