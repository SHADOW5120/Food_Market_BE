using Food_Market_BE.Modules.OrderModule.Models;

namespace Food_Market_BE.Modules.OrderModule.Helpers
{
    public static class OrderCalculationHelper
    {
        public static void CalculateItemSubtotal(OrderItem item)
        {
            decimal optionsTotal = item.Options.Sum(o => o.PriceModifier);
            item.Subtotal = (item.Price + optionsTotal) * item.Quantity;
        }

        public static decimal CalculateSubtotal(List<OrderItem> items)
        {
            return items.Sum(x => x.Subtotal);
        }

        public static decimal CalculateDiscount(decimal subtotal, string? voucherCode)
        {
            if (string.IsNullOrWhiteSpace(voucherCode))
                return 0;

            // Demo voucher logic
            if (voucherCode.Trim().ToUpper() == "DISCOUNT10")
                return subtotal * 0.10m;

            if (voucherCode.Trim().ToUpper() == "DISCOUNT20")
                return subtotal * 0.20m;

            return 0;
        }

        public static decimal CalculateShippingFee(decimal subtotal)
        {
            if (subtotal >= 200000)
                return 0;

            return 20000;
        }

        public static decimal CalculateTotal(decimal subtotal, decimal discount, decimal shippingFee)
        {
            return subtotal - discount + shippingFee;
        }
    }
}
