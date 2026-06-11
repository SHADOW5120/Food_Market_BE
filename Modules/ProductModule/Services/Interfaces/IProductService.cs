using Food_Market_BE.Modules.ProductModule.DTOs.Options;
using Food_Market_BE.Modules.ProductModule.DTOs.Product;

namespace Food_Market_BE.Modules.ProductModule.Services.Interfaces
{
    public interface IProductService
    {
        // =========================
        // PUBLIC
        // =========================

        Task<PagedProductResponse> GetProductsAsync(GetProductQueryDto query);

        Task<ProductDetailDto?> GetByIdAsync(string id);

        Task<PagedProductResponse> GetStoreProductsAsync(string storeId, GetProductQueryDto query);

        Task<PagedProductResponse> GetSellerProductsAsync(string sellerId, GetProductQueryDto query);

        // =========================
        // CUD (SECURED - SELLER + STORE)
        // =========================

        Task<ProductDetailDto> CreateAsync(string sellerId, string storeId, CreateProductRequest request);

        Task<bool> UpdateAsync(string sellerId, string storeId, string productId, UpdateProductRequest request);

        Task<bool> DeleteAsync(string sellerId, string storeId, string productId);

        Task<bool> ToggleAvailabilityAsync(string sellerId, string storeId, string productId);

        // =========================
        // OPTIONS (SECURED - SELLER + STORE + PRODUCT)
        // =========================

        Task<bool> AddOptionAsync(string sellerId, string storeId, string productId, CreateProductOptRequest request);

        Task<bool> UpdateOptionAsync(string sellerId, string storeId, string productId, string optionId, CreateProductOptRequest request);

        Task<bool> DeleteOptionAsync(string sellerId, string storeId, string productId, string optionId);

        Task<List<ProductOptDto>> GetOptionsByProductIdAsync(string productId);
    }
}