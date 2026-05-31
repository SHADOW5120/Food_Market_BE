using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.StoreModule.Controllers
{
    [ApiController]
    [Route("api/stores")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // =========================
        // PUBLIC
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetStores(
            [FromQuery] GetStoreQueryDto query)
        {
            var result = await _storeService.GetStoresAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStoreById(string id)
        {
            var result = await _storeService.GetStoreByIdAsync(id);

            if (result == null)
                return NotFound(new
                {
                    message = "Store not found"
                });

            return Ok(result);
        }

        [HttpGet("seller/{sellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStoreBySellerId(
            string sellerId,
            [FromQuery] GetStoreQueryDto query)
        {
            var result = await _storeService.GetStoreBySellerIdAsync(
                sellerId,
                query);

            return Ok(result);
        }

        // =========================
        // CREATE
        // =========================

        [HttpPost]
        [Authorize(Roles = UserRole.Seller + "," + UserRole.Admin)]
        public async Task<IActionResult> CreateStore(
            [FromBody] CreateStoreRequest request)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _storeService.CreateStoreAsync(
                request,
                userId);

            return Ok(result);
        }

        // =========================
        // UPDATE
        // =========================

        [HttpPut("{id}")]
        [Authorize(Roles = UserRole.Seller + "," + UserRole.Admin)]
        public async Task<IActionResult> UpdateStore(
            string id,
            [FromBody] UpdateStoreRequest request)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _storeService.UpdateStoreAsync(
                id,
                request,
                userId);

            return Ok(result);
        }

        // =========================
        // DELETE
        // =========================

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRole.Seller + "," + UserRole.Admin)]
        public async Task<IActionResult> DeleteStore(string id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _storeService.DeleteStoreAsync(
                id,
                userId);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Store not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Store deleted successfully"
            });
        }
    }
}