using Food_Market_BE.Modules.CartModule.Dtos;
using Food_Market_BE.Modules.CartModule.Helpers;
using Food_Market_BE.Modules.CartModule.Models;
using Food_Market_BE.Modules.CartModule.Repositories.Interfaces;
using Food_Market_BE.Modules.CartModule.Services.Interfaces;
using Food_Market_BE.Modules.ProductModule.Models;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.CartModule.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMongoCollection<Product> _productCollection;

        public CartService(
            ICartRepository cartRepository,
            MongoDbContext database)
        {
            _cartRepository = cartRepository;
            _productCollection = database.GetCollection<Product>("Products");
        }

        public async Task<CartResponse> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return new CartResponse
                {
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0
                };
            }

            return MapToResponse(cart);
        }

        public async Task<CartResponse> AddToCartAsync(string userId, AddToCartRequest request)
        {
            var product = await _productCollection
                .Find(x => x.Id == request.ProductId && x.IsAvailable && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new Exception("Product not found or unavailable.");
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>(),
                    TotalPrice = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _cartRepository.CreateAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.Price = product.Price;
                existingItem.ProductName = product.Name;
                existingItem.ProductImage = product.ImageUrl;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.ImageUrl,
                    Price = product.Price,
                    Quantity = request.Quantity
                });
            }

            CartCalculationHelper.RecalculateCart(cart);
            await _cartRepository.UpdateAsync(cart);

            return MapToResponse(cart);
        }

        public async Task<CartResponse> UpdateCartItemAsync(string userId, string productId, UpdateCartItemRequest request)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                throw new Exception("Cart not found.");
            }

            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                throw new Exception("Item not found in cart.");
            }

            if (request.Quantity == 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                var product = await _productCollection
                    .Find(x => x.Id == productId && x.IsAvailable && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    throw new Exception("Product not found or unavailable.");
                }

                item.Quantity = request.Quantity;
                item.Price = product.Price;
                item.ProductName = product.Name;
                item.ProductImage = product.ImageUrl;
            }

            CartCalculationHelper.RecalculateCart(cart);
            await _cartRepository.UpdateAsync(cart);

            return MapToResponse(cart);
        }

        public async Task<CartResponse> RemoveItemAsync(string userId, string productId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                throw new Exception("Cart not found.");
            }

            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                throw new Exception("Item not found in cart.");
            }

            cart.Items.Remove(item);

            CartCalculationHelper.RecalculateCart(cart);
            await _cartRepository.UpdateAsync(cart);

            return MapToResponse(cart);
        }

        public async Task<CartResponse> ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>(),
                    TotalPrice = 0
                };

                await _cartRepository.CreateAsync(cart);
            }
            else
            {
                cart.Items.Clear();
                CartCalculationHelper.RecalculateCart(cart);
                await _cartRepository.UpdateAsync(cart);
            }

            return MapToResponse(cart);
        }

        public async Task<string> CheckoutAsync(string userId, CheckoutRequest request)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null || !cart.Items.Any())
            {
                throw new Exception("Cart is empty.");
            }

            return "Checkout flow will be implemented in OrderModule.";
        }

        private CartResponse MapToResponse(Cart cart)
        {
            return new CartResponse
            {
                Items = cart.Items.Select(x => new CartItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductImage = x.ProductImage,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Subtotal = x.Subtotal
                }).ToList(),
                TotalPrice = cart.TotalPrice
            };
        }
    }
}
