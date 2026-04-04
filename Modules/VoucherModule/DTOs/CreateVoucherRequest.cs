using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.VoucherModule.DTOs
{
    public class CreateVoucherRequest
    {
        [Required]
        public string Code { get; set; } = default!;

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercent { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinOrderAmount { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal? MaxDiscountAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxUsage { get; set; }
    }
}
