using Food_Market_BE.Modules.VoucherModule.Models;

namespace Food_Market_BE.Modules.VoucherModule.Helpers
{
    public static class VoucherHelper
    {
        public static bool IsVoucherExpired(Voucher voucher)
        {
            return voucher.ExpiryDate < DateTime.UtcNow;
        }

        public static bool IsEligibleOrderAmount(Voucher voucher, decimal cartTotal)
        {
            return cartTotal >= voucher.MinOrderAmount;
        }

        public static bool IsUsageExceeded(Voucher voucher)
        {
            if (!voucher.MaxUsage.HasValue) return false;
            return voucher.UsedCount >= voucher.MaxUsage.Value;
        }

        public static decimal CalculateDiscount(Voucher voucher, decimal cartTotal)
        {
            decimal discount = 0;

            if (voucher.DiscountAmount.HasValue && voucher.DiscountAmount.Value > 0)
            {
                discount = voucher.DiscountAmount.Value;
            }
            else if (voucher.DiscountPercent.HasValue && voucher.DiscountPercent.Value > 0)
            {
                discount = cartTotal * (voucher.DiscountPercent.Value / 100);
            }

            if (voucher.MaxDiscountAmount.HasValue && discount > voucher.MaxDiscountAmount.Value)
            {
                discount = voucher.MaxDiscountAmount.Value;
            }

            if (discount > cartTotal)
            {
                discount = cartTotal;
            }

            return discount;
        }
    }
}
