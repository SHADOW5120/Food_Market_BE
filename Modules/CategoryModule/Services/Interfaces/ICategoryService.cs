using Food_Market_BE.Modules.CategoryModule.DTOs;

namespace Food_Market_BE.Modules.CategoryModule.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(string name);
    }
}
