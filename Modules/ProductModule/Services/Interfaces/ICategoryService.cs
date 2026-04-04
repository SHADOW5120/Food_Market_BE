using Food_Market_BE.Modules.ProductModule.DTOs;

namespace Food_Market_BE.Modules.ProductModule.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(string name);
    }
}
