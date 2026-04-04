namespace Food_Market_BE.Modules.VoucherModule.DTOs
{
    public class ApplyVoucherResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = default!;

        public string? VoucherCode { get; set; }
        public decimal CartTotal { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FinalTotal { get; set; }
    }
}
