using Food_Market_BE.Modules.ProductModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Modules.ProductModule.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] string name)
        {
            var category = await _categoryService.CreateAsync(name);
            return Ok(category);
        }
    }
}