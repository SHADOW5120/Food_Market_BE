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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetStores([FromQuery] GetStoreQueryDto query)
        {
            var result = await _storeService.GetStoresAsync(
                query.Page,
                query.PageSize,
                query.Search,
                query.MinRating);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStoreById(string id)
        {
            var result = await _storeService.GetStoreDetailAsync(id);
            if (result == null)
                throw new Exception("Store not found");

            return Ok(result);
        }

        [HttpGet("{id}/products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStoreProducts(string id)
        {
            var result = await _storeService.GetStoreProductsAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Seller + "," + UserRole.Admin)]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
                throw new Exception("Unauthorized");

            var result = await _storeService.CreateStoreAsync(request, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRole.Seller + "," + UserRole.Admin)]
        public async Task<IActionResult> UpdateStore(string id, [FromBody] UpdateStoreRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
                throw new Exception("Unauthorized");

            var result = await _storeService.UpdateStoreAsync(id, request, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRole.Seller + "," + UserRole.Admin)]
        public async Task<IActionResult> DeleteStore(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
                throw new Exception("Unauthorized");

            var result = await _storeService.DeleteStoreAsync(id, userId);
            return Ok(new
            {
                success = result,
                message = "Store deleted successfully"
            });
        }
    }
}