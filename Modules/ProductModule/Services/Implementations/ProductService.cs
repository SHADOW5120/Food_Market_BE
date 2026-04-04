using Food_Market_BE.Modules.ProductModule.DTOs;
using Food_Market_BE.Modules.ProductModule.Helpers;
using Food_Market_BE.Modules.ProductModule.Models;
using Food_Market_BE.Modules.ProductModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ProductModule.Services.Interfaces;

namespace Food_Market_BE.Modules.ProductModule.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedProductResponse> GetProductsAsync(GetProductsQueryDto query)
        {
            var products = await _productRepository.GetAllAsync();

            products = products
                .Where(x => !x.IsDeleted && x.IsAvailable)
                .ToList();

            if (!string.IsNullOrWhiteSpace(query.CategoryId))
            {
                products = products
                    .Where(x => x.CategoryId == query.CategoryId)
                    .ToList();
            }

            if (query.MinPrice.HasValue)
            {
                products = products
                    .Where(x => x.Price >= query.MinPrice.Value)
                    .ToList();
            }

            if (query.MaxPrice.HasValue)
            {
                products = products
                    .Where(x => x.Price <= query.MaxPrice.Value)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                products = products
                    .Where(x => x.Name.ToLower().Contains(query.Search.ToLower()))
                    .ToList();
            }

            var sortedProducts = ProductSortHelper.ApplySort(products, query.Sort).ToList();

            var total = sortedProducts.Count;

            var pagedItems = sortedProducts
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    IsAvailable = x.IsAvailable
                })
                .ToList();

            return new PagedProductResponse
            {
                Items = pagedItems,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ProductDetailDto?> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null || product.IsDeleted || !product.IsAvailable)
                return null;

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                CreatedAt = product.CreatedAt,
                Category = new CategoryDto
                {
                    Id = category?.Id ?? "",
                    Name = category?.Name ?? ""
                }
            };
        }

        public async Task<ProductDetailDto> CreateAsync(string sellerId, CreateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Product name is required");

            if (request.Price <= 0)
                throw new Exception("Price must be greater than 0");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new Exception("Category not found");

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId,
                SellerId = sellerId,
                IsAvailable = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productRepository.CreateAsync(product);

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                CreatedAt = product.CreatedAt,
                Category = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                }
            };
        }

        public async Task<bool> UpdateAsync(string sellerId, string productId, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            if (product.SellerId != sellerId)
                return false;

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Product name is required");

            if (request.Price <= 0)
                throw new Exception("Price must be greater than 0");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new Exception("Category not found");

            product.Name = request.Name.Trim();
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.CategoryId = request.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(string sellerId, string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            if (product.SellerId != sellerId)
                return false;

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> ToggleAvailabilityAsync(string sellerId, string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            if (product.SellerId != sellerId)
                return false;

            product.IsAvailable = !product.IsAvailable;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }
    }
}
