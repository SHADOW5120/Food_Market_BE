using Food_Market_BE.Modules.ProductModule.DTOs.Options;
using Food_Market_BE.Modules.ProductModule.DTOs.Product;

namespace Food_Market_BE.Modules.ProductModule.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedProductResponse> GetProductsAsync(GetProductQueryDto query);
        Task<PagedProductResponse> GetSellerProductsAsync(string sellerId, GetProductQueryDto query);
        Task<PagedProductResponse> GetStoreProductsAsync(string storeId, GetProductQueryDto query);
        Task<ProductDetailDto?> GetByIdAsync(string id);
        Task<ProductDetailDto> CreateAsync(string sellerId, CreateProductRequest request);
        Task<bool> UpdateAsync(string sellerId, string productId, UpdateProductRequest request);
        Task<bool> DeleteAsync(string sellerId, string productId);
        Task<bool> ToggleAvailabilityAsync(string sellerId, string productId);

        Task<bool> AddOptionAsync(string productId, CreateProductOptRequest request);
        Task<bool> UpdateOptionAsync(string productId, string optionId, CreateProductOptRequest request);
        Task<bool> DeleteOptionAsync(string productId, string optionId);
        Task<List<ProductOptDto>> GetOptionsByProductIdAsync(string productId);
    }
}
