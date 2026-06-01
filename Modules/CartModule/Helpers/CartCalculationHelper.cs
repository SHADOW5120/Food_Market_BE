using Food_Market_BE.Modules.CartModule.Models;

namespace Food_Market_BE.Modules.CartModule.Helpers
{
    public static class CartCalculationHelper
    {
        public static void RecalculateCart(Cart cart)
        {
            if (cart.Items == null || !cart.Items.Any())
            {
                cart.Items = new List<CartItem>();
                cart.TotalPrice = 0m;
                cart.UpdatedAt = DateTime.UtcNow;
                return;
            }

            foreach (var item in cart.Items)
            {
                decimal optionsTotal = item.Options?.Sum(o => o.PriceModifier) ?? 0m;
                item.Subtotal = (item.Price + optionsTotal) * item.Quantity;
            }

            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);
            cart.UpdatedAt = DateTime.UtcNow;
        }
    }
}
