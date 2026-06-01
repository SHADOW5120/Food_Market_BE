using Food_Market_BE.Modules.CartModule.Models;
using Food_Market_BE.Modules.CartModule.Repositories.Interfaces;
using Food_Market_BE.Modules.OrderModule.DTOs;
using Food_Market_BE.Modules.OrderModule.Helpers;
using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Modules.OrderModule.Repositories.Interfaces;
using Food_Market_BE.Modules.OrderModule.Services.Interfaces;

namespace Food_Market_BE.Modules.OrderModule.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        //private readonly IMongoCollection<Cart> _carts;
        private readonly ICartRepository _cartRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            //_carts = database.GetCollection<Cart>("Carts");
            _cartRepository = cartRepository;
        }

        public async Task<OrderDetailDto> CreateOrderAsync(CreateOrderRequest request, string userId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.CartId)) throw new ArgumentException("CartId is required", nameof(request.CartId));

            var cart = await _cartRepository.GetByIdAndUserIdAsync(request.CartId, userId);

            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            if (cart.Items == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            var orderItems = cart.Items.Select(item =>
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    UnitPrice = item.Price,
                    Quantity = item.Quantity,
                    Options = item.Options.Select(opt => new OrderItemOpt
                    {
                        OptionId = opt.OptionId,
                        OptionName = opt.OptionName,
                        ValueId = opt.ValueId,
                        ValueName = opt.ValueName,
                        PriceModifier = opt.PriceModifier
                    }).ToList()
                };

                OrderCalculationHelper.CalculateItemSubtotal(orderItem);
                return orderItem;
            }).ToList();

            var subtotal = OrderCalculationHelper.CalculateSubtotal(orderItems);
            var discount = OrderCalculationHelper.CalculateDiscount(subtotal, request.VoucherCode);
            var shippingFee = OrderCalculationHelper.CalculateShippingFee(subtotal);
            var total = OrderCalculationHelper.CalculateTotal(subtotal, discount, shippingFee);

            var order = new Order
            {
                UserId = userId,
                //CartId = request.CartId,
                Items = orderItems,
                Subtotal = subtotal,
                DiscountAmount = discount,
                ShippingFee = shippingFee,
                TotalPrice = total,
                Status = OrderStatus.Pending,
                DeliveryAddress = request.DeliveryAddress,
                PaymentMethod = request.PaymentMethod,
                VoucherCode = request.VoucherCode,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepository.CreateAsync(order);

            // Clear cart after checkout
            cart.Items.Clear();
            cart.TotalPrice = 0m;
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);

            return MapToDetailDto(order);
        }

        public async Task<PagedOrderResponse> GetUserOrdersAsync(string userId, int page, int pageSize)
        {
            var (orders, totalCount) = await _orderRepository.GetByUserIdAsync(userId, page, pageSize);

            return new PagedOrderResponse
            {
                Items = orders.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderDetailDto?> GetOrderDetailAsync(string orderId, string userId, bool isAdmin = false)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return null;

            if (!isAdmin && order.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to view this order.");

            return MapToDetailDto(order);
        }

        public async Task<bool> CancelOrderAsync(string orderId, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to cancel this order.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled.");

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (!IsValidStatusTransition(order.Status, newStatus))
                throw new InvalidOperationException($"Cannot change status from '{order.Status}' to '{newStatus}'.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == OrderStatus.Cancelled)
                order.CancelledAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<PagedOrderResponse> GetSellerOrdersAsync(string sellerId, int page, int pageSize)
        {
            // This is a placeholder implementation
            // In a real scenario, you would filter orders by seller's products
            var orders = await _orderRepository.GetAllAsync(page, pageSize);

            // Filter orders that contain products from this seller
            // Note: This requires linking orders to products and checking the seller
            var sellerOrders = orders.Where(o => 
                o.Items.Any() // In a real implementation, check if items belong to this seller
            ).ToList();

            return new PagedOrderResponse
            {
                Items = sellerOrders.Select(MapToResponseDto).ToList(),
                TotalCount = sellerOrders.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderDetailDto?> GetSellerOrderDetailAsync(string orderId, string sellerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return null;

            // In a real implementation, verify that the seller owns the products in this order
            return MapToDetailDto(order);
        }

        public async Task<bool> UpdateSellerOrderStatusAsync(string orderId, string sellerId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            // In a real implementation, verify that the seller owns the products in this order
            if (!IsValidStatusTransition(order.Status, newStatus))
                throw new InvalidOperationException($"Cannot change status from '{order.Status}' to '{newStatus}'.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == OrderStatus.Cancelled)
                order.CancelledAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            if (currentStatus == OrderStatus.Cancelled || currentStatus == OrderStatus.Completed)
                return false;

            return currentStatus switch
            {
                OrderStatus.Pending => newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled,
                OrderStatus.Confirmed => newStatus is OrderStatus.Delivering or OrderStatus.Cancelled,
                OrderStatus.Delivering => newStatus == OrderStatus.Completed,
                _ => false
            };
        }

        private OrderResponse MapToResponseDto(Order order)
        {
            return new OrderResponse
            {
                OrderId = order.Id,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductImage = x.ProductImage,
                    Price = x.UnitPrice,
                    Quantity = x.Quantity,
                    Options = x.Options.Select(o => new OrderItemOptDto
                    {
                        OptionId = o.OptionId,
                        OptionName = o.OptionName,
                        ValueId = o.ValueId,
                        ValueName = o.ValueName,
                        PriceModifier = o.PriceModifier
                    }).ToList(),
                    Subtotal = x.Subtotal
                }).ToList()
            };
        }

        private OrderDetailDto MapToDetailDto(Order order)
        {
            return new OrderDetailDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                //CartId = order.CartId,
                Status = order.Status,
                DeliveryAddress = order.DeliveryAddress,
                PaymentMethod = order.PaymentMethod,
                VoucherCode = order.VoucherCode,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                CancelledAt = order.CancelledAt,
                Items = order.Items.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductImage = x.ProductImage,
                    Price = x.UnitPrice,
                    Quantity = x.Quantity,
                    Options = x.Options.Select(o => new OrderItemOptDto
                    {
                        OptionId = o.OptionId,
                        OptionName = o.OptionName,
                        ValueId = o.ValueId,
                        ValueName = o.ValueName,
                        PriceModifier = o.PriceModifier
                    }).ToList(),
                    Subtotal = x.Subtotal
                }).ToList()
            };
        }
    }
}
