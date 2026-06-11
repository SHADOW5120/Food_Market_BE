using Food_Market_BE.Modules.ProductModule.DTOs.Options;
using Food_Market_BE.Modules.ProductModule.DTOs.Product;
using Food_Market_BE.Modules.ProductModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.ProductModule.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // =========================
        // PUBLIC PRODUCTS
        // =========================

        [HttpGet("api/products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductQueryDto query)
        {
            var result = await _productService.GetProductsAsync(query);
            return Ok(result);
        }

        [HttpGet("api/products/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(string id)
        {
            var result = await _productService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Product not found" });

            return Ok(result);
        }

        [HttpGet("api/stores/{storeId}/products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByStore(string storeId, [FromQuery] GetProductQueryDto query)
        {
            var result = await _productService.GetStoreProductsAsync(storeId, query);
            return Ok(result);
        }

        [HttpGet("api/products/{productId}/options")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductOptions(string productId)
        {
            var result = await _productService.GetOptionsByProductIdAsync(productId);
            return Ok(result);
        }

        // =========================
        // SELLER - OWN STORE PRODUCTS
        // =========================

        [HttpGet("api/stores/{storeId}/products/my")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> GetMyStoreProducts(string storeId, [FromQuery] GetProductQueryDto query)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.GetSellerProductsAsync(sellerId, query);

            return Ok(result);
        }

        [HttpPost("api/stores/{storeId}/products")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> CreateProduct(string storeId, [FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.CreateAsync(sellerId, storeId, request);

            return Ok(result);
        }

        [HttpPut("api/stores/{storeId}/products/{productId}")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> UpdateProduct(string storeId, string productId, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.UpdateAsync(sellerId, storeId, productId, request);

            if (!result)
                return NotFound(new { message = "Product not found or unauthorized" });

            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("api/stores/{storeId}/products/{productId}")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> DeleteProduct(string storeId, string productId)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.DeleteAsync(sellerId, storeId, productId);

            if (!result)
                return NotFound(new { message = "Product not found or unauthorized" });

            return Ok(new { message = "Deleted successfully" });
        }

        [HttpPatch("api/stores/{storeId}/products/{productId}/toggle")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> ToggleProduct(string storeId, string productId)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.ToggleAvailabilityAsync(sellerId, storeId, productId);

            if (!result)
                return NotFound(new { message = "Product not found or unauthorized" });

            return Ok(new { message = "Status updated" });
        }

        // =========================
        // PRODUCT OPTIONS
        // =========================

        [HttpPost("api/stores/{storeId}/products/{productId}/options")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> AddOption(string storeId, string productId, [FromBody] CreateProductOptRequest request)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.AddOptionAsync(sellerId, storeId, productId, request);

            if (!result)
                return NotFound(new { message = "Product not found or unauthorized" });

            return Ok(new { message = "Option added" });
        }

        [HttpPut("api/stores/{storeId}/products/{productId}/options/{optionId}")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> UpdateOption(string storeId, string productId, string optionId, [FromBody] CreateProductOptRequest request)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.UpdateOptionAsync(sellerId, storeId, productId, optionId, request);

            if (!result)
                return NotFound(new { message = "Option not found or unauthorized" });

            return Ok(new { message = "Option updated" });
        }

        [HttpDelete("api/stores/{storeId}/products/{productId}/options/{optionId}")]
        [Authorize(Roles = UserRole.Seller)]
        public async Task<IActionResult> DeleteOption(string storeId, string productId, string optionId)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
                return Unauthorized();

            var result = await _productService.DeleteOptionAsync(sellerId, storeId, productId, optionId);

            if (!result)
                return NotFound(new { message = "Option not found or unauthorized" });

            return Ok(new { message = "Option deleted" });
        }
    }
}