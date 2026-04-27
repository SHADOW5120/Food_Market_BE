using Food_Market_BE.Modules.ProductModule.Models.Product;

namespace Food_Market_BE.Modules.ProductModule.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(string id);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
    }
}
