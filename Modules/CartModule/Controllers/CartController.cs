using Food_Market_BE.Modules.CartModule.Dtos;
using Food_Market_BE.Modules.CartModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.CartModule.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var result = await _cartService.GetCartAsync(userId);

            return Ok(new
            {
                success = true,
                message = "Cart retrieved successfully.",
                data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = GetUserId();
            var result = await _cartService.AddToCartAsync(userId, request);

            return Ok(new
            {
                success = true,
                message = "Added to cart successfully.",
                data = result
            });
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem(string productId, [FromBody] UpdateCartItemRequest request)
        {
            var userId = GetUserId();
            var result = await _cartService.UpdateCartItemAsync(userId, productId, request);

            return Ok(new
            {
                success = true,
                message = "Cart item updated successfully.",
                data = result
            });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem(string productId)
        {
            var userId = GetUserId();
            var result = await _cartService.RemoveItemAsync(userId, productId);

            return Ok(new
            {
                success = true,
                message = "Item removed from cart successfully.",
                data = result
            });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var result = await _cartService.ClearCartAsync(userId);

            return Ok(new
            {
                success = true,
                message = "Cart cleared successfully.",
                data = result
            });
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var userId = GetUserId();
            var result = await _cartService.CheckoutAsync(userId, request);

            return Ok(new
            {
                success = true,
                message = result
            });
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("id")?.Value
                   ?? throw new UnauthorizedAccessException("User ID not found in token.");
        }
    }
}
