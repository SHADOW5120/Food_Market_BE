using Food_Market_BE.Modules.CartModule.Dtos;
using Food_Market_BE.Modules.CartModule.Helpers;
using Food_Market_BE.Modules.CartModule.Models;
using Food_Market_BE.Modules.CartModule.Repositories.Interfaces;
using Food_Market_BE.Modules.CartModule.Services.Interfaces;
using Food_Market_BE.Modules.ProductModule.Repositories.Interfaces;

namespace Food_Market_BE.Modules.CartModule.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        //private readonly IMongoCollection<Product> _productCollection;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            //MongoDbContext database,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            //_productCollection = database.GetCollection<Product>("Products");
            _productRepository = productRepository;
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
            if (request == null) throw new ArgumentNullException(nameof(request));

            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if (product == null)
            {
                throw new InvalidOperationException("Product not found or unavailable.");
            }

            // Validate selected options
            var cartItemOpts = new List<CartItemOpt>();
            if (request.SelectedOptions.Any())
            {
                foreach (var selectedOpt in request.SelectedOptions)
                {
                    var productOpt = product.Options.FirstOrDefault(o => o.Id == selectedOpt.OptionId);
                        if (productOpt == null)
                        {
                            throw new ArgumentException($"Option {selectedOpt.OptionId} not found for this product.");
                        }

                    var value = productOpt.Values.FirstOrDefault(v => v.Id == selectedOpt.ValueId);
                    if (value == null)
                    {
                        throw new ArgumentException($"Value {selectedOpt.ValueId} not found for option {selectedOpt.OptionId}.");
                    }

                    cartItemOpts.Add(new CartItemOpt
                    {
                        OptionId = selectedOpt.OptionId,
                        ValueId = selectedOpt.ValueId,
                        OptionName = productOpt.Name,
                        ValueName = value.Name,
                        PriceModifier = value.PriceModifier,
                        CreatedAt = DateTime.UtcNow
                    });
                }
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

            // Check if item with same product and same options already exists
            var existingItem = cart.Items.FirstOrDefault(x => 
                x.ProductId == request.ProductId && 
                OptionsMatch(x.Options, cartItemOpts));

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                    Price = product.Price,
                    Quantity = request.Quantity,
                    Options = cartItemOpts
                });
            }

            CartCalculationHelper.RecalculateCart(cart);
            await _cartRepository.UpdateAsync(cart);

            return MapToResponse(cart);
        }

        private bool OptionsMatch(List<CartItemOpt> existingOpts, List<CartItemOpt> newOpts)
        {
            existingOpts = existingOpts ?? new List<CartItemOpt>();
            newOpts = newOpts ?? new List<CartItemOpt>();

            if (existingOpts.Count != newOpts.Count)
                return false;

            foreach (var newOpt in newOpts)
            {
                var existingOpt = existingOpts.FirstOrDefault(o => o.OptionId == newOpt.OptionId);
                if (existingOpt == null || existingOpt.ValueId != newOpt.ValueId)
                    return false;
            }

            return true;
        }

        public async Task<CartResponse> UpdateCartItemAsync(string userId, string productId, UpdateCartItemRequest request)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                throw new InvalidOperationException("Cart not found.");
            }

            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                throw new InvalidOperationException("Item not found in cart.");
            }

            if (request.Quantity == 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                var product = await _productRepository.GetByIdAsync(productId);

                if (product == null)
                {
                    throw new InvalidOperationException("Product not found or unavailable.");
                }

                item.Quantity = request.Quantity;
                item.Price = product.Price;
                item.ProductName = product.Name;
                item.ProductImage = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl;
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
                throw new InvalidOperationException("Cart not found.");
            }

            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                throw new InvalidOperationException("Item not found in cart.");
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
                    //TotalPrice = 0
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
                throw new InvalidOperationException("Cart is empty.");
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
                    Options = x.Options.Select(o => new CartItemOptDto
                    {
                        OptionId = o.OptionId,
                        ValueId = o.ValueId,
                        OptionName = o.OptionName,
                        ValueName = o.ValueName,
                        PriceModifier = o.PriceModifier
                    }).ToList(),
                    Subtotal = x.Subtotal
                }).ToList(),
                TotalPrice = cart.TotalPrice
            };
        }
    }
}
