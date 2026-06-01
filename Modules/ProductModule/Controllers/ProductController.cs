using Food_Market_BE.Modules.ProductModule.DTOs.Product;
using Food_Market_BE.Modules.ProductModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.ProductModule.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductQueryDto query)
        {
            var result = await _productService.GetProductsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Product not found" });

            return Ok(result);
        }

        [Authorize(Roles = UserRole.Seller)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? throw new UnauthorizedAccessException("Unauthorized");

            var result = await _productService.CreateAsync(sellerId, request);
            return Ok(result);
        }

        [Authorize(Roles = UserRole.Seller)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? throw new UnauthorizedAccessException("Unauthorized");

            var success = await _productService.UpdateAsync(sellerId, id, request);

            if (!success)
                return NotFound(new { message = "Product not found or unauthorized" });

            return Ok(new { message = "Updated successfully" });
        }

        [Authorize(Roles = UserRole.Seller)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? throw new UnauthorizedAccessException("Unauthorized");

            var success = await _productService.DeleteAsync(sellerId, id);

            if (!success)
                return NotFound(new { message = "Product not found or unauthorized" });

            return Ok(new { message = "Deleted successfully" });
        }

        [Authorize(Roles = UserRole.Seller)]
        [HttpPatch("{id}/toggle-availability")]
        public async Task<IActionResult> ToggleAvailability(string id)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? throw new UnauthorizedAccessException("Unauthorized");

            var success = await _productService.ToggleAvailabilityAsync(sellerId, id);

            if (!success)
                return NotFound(new { message = "Product not found or unauthorized" });

            return Ok(new { message = "Availability updated" });
        }
    }
}