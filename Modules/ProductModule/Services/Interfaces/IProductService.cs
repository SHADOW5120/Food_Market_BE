using Food_Market_BE.Modules.ProductModule.DTOs;

namespace Food_Market_BE.Modules.ProductModule.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedProductResponse> GetProductsAsync(GetProductsQueryDto query);
        Task<PagedProductResponse> GetSellerProductsAsync(string sellerId, GetProductsQueryDto query);
        Task<ProductDetailDto?> GetByIdAsync(string id);
        Task<ProductDetailDto> CreateAsync(string sellerId, CreateProductRequest request);
        Task<bool> UpdateAsync(string sellerId, string productId, UpdateProductRequest request);
        Task<bool> DeleteAsync(string sellerId, string productId);
        Task<bool> ToggleAvailabilityAsync(string sellerId, string productId);
    }
}
