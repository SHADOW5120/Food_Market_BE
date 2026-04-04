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
                cart.TotalPrice = 0;
                cart.UpdatedAt = DateTime.UtcNow;
                return;
            }

            foreach (var item in cart.Items)
            {
                item.Subtotal = item.Price * item.Quantity;
            }

            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);
            cart.UpdatedAt = DateTime.UtcNow;
        }
    }
}
