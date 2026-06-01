using Food_Market_BE.Modules.CategoryModule.DTOs;
using Food_Market_BE.Modules.CategoryModule.Models;
using Food_Market_BE.Modules.CategoryModule.Repositories.Interfaces;
using Food_Market_BE.Modules.CategoryModule.Services.Interfaces;

namespace Food_Market_BE.Modules.CategoryModule.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

        public async Task<CategoryDto> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required", nameof(name));

            var category = new Category
            {
                Name = name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.CreateAsync(category);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
