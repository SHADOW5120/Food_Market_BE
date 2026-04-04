using Food_Market_BE.Modules.ProductModule.Models;

namespace Food_Market_BE.Modules.ProductModule.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(string id);
        Task CreateAsync(Category category);
    }
}
