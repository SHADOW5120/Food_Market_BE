using Food_Market_BE.Modules.CategoryModule.Models;

namespace Food_Market_BE.Modules.CategoryModule.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(string id);
        Task CreateAsync(Category category);
    }
}
