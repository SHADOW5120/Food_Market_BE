using Food_Market_BE.Modules.ProductModule.DTOs;
using Food_Market_BE.Modules.ProductModule.Models;
using Food_Market_BE.Modules.ProductModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ProductModule.Services.Interfaces;

namespace Food_Market_BE.Modules.ProductModule.Services.Implementations
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

            return categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }

        public async Task<CategoryDto> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Category name is required");

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
