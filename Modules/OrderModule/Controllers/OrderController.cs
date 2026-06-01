using Food_Market_BE.Modules.OrderModule.DTOs;
using Food_Market_BE.Modules.OrderModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.OrderModule.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = GetUserId();
            var result = await _orderService.CreateOrderAsync(request, userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var result = await _orderService.GetUserOrdersAsync(userId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetail(string id)
        {
            var userId = GetUserId();
            var result = await _orderService.GetOrderDetailAsync(id, userId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(string id)
        {
            var userId = GetUserId();
            var success = await _orderService.CancelOrderAsync(id, userId);
            return Ok(new { cancelled = success });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var success = await _orderService.UpdateOrderStatusAsync(id, request.NewStatus);
            return Ok(new { updated = success });
        }

        [HttpGet("seller")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> GetSellerOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var sellerId = GetUserId();

            var result = await _orderService
                .GetSellerOrdersAsync(
                    sellerId,
                    page,
                    pageSize);

            return Ok(result);
        }

        [HttpGet("seller/{id}")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> GetSellerOrderDetail(
    string id)
        {
            var sellerId = GetUserId();

            var result =
                await _orderService.GetSellerOrderDetailAsync(
                    id,
                    sellerId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("seller/{id}/status")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> UpdateSellerOrderStatus(
    string id,
    [FromBody] UpdateOrderStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var sellerId = GetUserId();

            var result =
                await _orderService.UpdateSellerOrderStatusAsync(
                    id,
                    sellerId,
                    request.NewStatus);

            return Ok(new
            {
                success = result
            });
        }



        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("id")
                       ?? throw new UnauthorizedAccessException("UserId not found in token.");
        }


    }
}