using Food_Market_BE.Modules.ProductModule.DTOs.Product;
using Food_Market_BE.Modules.ProductModule.Models.Product;

namespace Food_Market_BE.Modules.ProductModule.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(string id);
        Task<List<Product>> GetByStoreIdAsync(string storeId);
        Task<List<Product>> GetByCategoryIdAsync(string categoryId);
        Task<List<Product>> GetBySellerIdAsync(string sellerId);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);

        Task<List<Product>> SearchAsync(GetProductQueryDto query);
    }
}
