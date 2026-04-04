namespace Food_Market_BE.Modules.VoucherModule.DTOs
{
    public class VoucherDto
    {
        public string VoucherId { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? Description { get; set; }

        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }

        public DateTime ExpiryDate { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
    }
}
