using Food_Market_BE.Modules.CategoryModule.DTOs;
using Food_Market_BE.Modules.CategoryModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ProductModule.DTOs.Media;
using Food_Market_BE.Modules.ProductModule.DTOs.Options;
using Food_Market_BE.Modules.ProductModule.DTOs.Product;
using Food_Market_BE.Modules.ProductModule.Models.Media;
using Food_Market_BE.Modules.ProductModule.Models.Options;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.ProductModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ProductModule.Services.Interfaces;
using Food_Market_BE.Modules.StoreModule.Repositories.Interfaces;

namespace Food_Market_BE.Modules.ProductModule.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStoreRepository _storeRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IStoreRepository storeRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _storeRepository = storeRepository;
        }

        // =========================
        // HELPERS
        // =========================

        private async Task<bool> ValidateStoreOwnership(string sellerId, string storeId)
        {
            var store = await _storeRepository.GetStoreByIdAsync(storeId);

            if (store == null)
                return false;

            if (store.OwnerId != sellerId)
                return false;

            return true;
        }

        private async Task<Product?> GetOwnedProduct(string sellerId, string storeId, string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return null;

            if (product.StoreId != storeId)
                return null;

            var ok = await ValidateStoreOwnership(sellerId, storeId);

            if (!ok)
                return null;

            return product;
        }

        // =========================
        // PUBLIC
        // =========================

        public async Task<PagedProductResponse> GetProductsAsync(GetProductQueryDto query)
        {
            var products = await _productRepository.SearchAsync(query);

            return MapPaged(products, query);
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
                IsAvailable = product.IsAvailable,
                CreatedAt = product.CreatedAt,
                Images = product.Images?.Select(i => new ProductImgDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary
                }).ToList() ?? new(),
                Options = product.Options?.Select(o => new ProductOptDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    IsRequired = o.IsRequired,
                    IsMultiple = o.IsMultiple,
                    Values = o.Values?.Select(v => new ProductOptValueDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        PriceModifier = v.PriceModifier
                    }).ToList() ?? new()
                }).ToList() ?? new(),
                Category = new CategoryDto
                {
                    Id = category?.Id ?? "",
                    Name = category?.Name ?? ""
                }
            };
        }

        public async Task<PagedProductResponse> GetStoreProductsAsync(string storeId, GetProductQueryDto query)
        {
            var products = await _productRepository.GetByStoreIdAsync(storeId);

            var filtered = ApplyCommonFilters(products, query);

            return MapPaged(filtered, query);
        }

        public async Task<PagedProductResponse> GetSellerProductsAsync(string sellerId, GetProductQueryDto query)
        {
            var products = await _productRepository.GetBySellerIdAsync(sellerId);

            var filtered = ApplyCommonFilters(products, query);

            return MapPaged(filtered, query);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<ProductDetailDto> CreateAsync(string sellerId, string storeId, CreateProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!await ValidateStoreOwnership(sellerId, storeId))
                throw new UnauthorizedAccessException("Not owner of store");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);

            if (category == null)
                throw new InvalidOperationException("Category not found");

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                StoreId = storeId,
                IsAvailable = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Images = request.Images?.Select(i => new ProductImg
                {
                    Id = Guid.NewGuid().ToString(),
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary,
                    CreatedAt = DateTime.UtcNow
                }).ToList() ?? new(),
                Options = request.Options?.Select(o => new ProductOpt
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = o.Name,
                    IsRequired = o.IsRequired,
                    IsMultiple = o.IsMultiple,
                    CreatedAt = DateTime.UtcNow,
                    Values = o.Values?.Select(v => new ProductOptValue
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = v.Name,
                        PriceModifier = v.PriceModifier,
                        CreatedAt = DateTime.UtcNow
                    }).ToList() ?? new()
                }).ToList() ?? new()
            };

            await _productRepository.CreateAsync(product);

            return (await GetByIdAsync(product.Id))!;
        }

        // =========================
        // UPDATE
        // =========================

        public async Task<bool> UpdateAsync(string sellerId, string storeId, string productId, UpdateProductRequest request)
        {
            var product = await GetOwnedProduct(sellerId, storeId, productId);

            if (product == null)
                return false;

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);

            if (category == null)
                throw new InvalidOperationException("Category not found");

            product.Name = request.Name.Trim();
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            return true;
        }

        // =========================
        // DELETE
        // =========================

        public async Task<bool> DeleteAsync(string sellerId, string storeId, string productId)
        {
            var product = await GetOwnedProduct(sellerId, storeId, productId);

            if (product == null)
                return false;

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            return true;
        }

        // =========================
        // TOGGLE
        // =========================

        public async Task<bool> ToggleAvailabilityAsync(string sellerId, string storeId, string productId)
        {
            var product = await GetOwnedProduct(sellerId, storeId, productId);

            if (product == null)
                return false;

            product.IsAvailable = !product.IsAvailable;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            return true;
        }

        // =========================
        // OPTIONS (SECURED)
        // =========================

        public async Task<bool> AddOptionAsync(string sellerId, string storeId, string productId, CreateProductOptRequest request)
        {
            var product = await GetOwnedProduct(sellerId, storeId, productId);

            if (product == null)
                return false;

            var option = new ProductOpt
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                IsRequired = request.IsRequired,
                IsMultiple = request.IsMultiple,
                CreatedAt = DateTime.UtcNow,
                Values = request.Values?.Select(v => new ProductOptValue
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = v.Name,
                    PriceModifier = v.PriceModifier,
                    CreatedAt = DateTime.UtcNow
                }).ToList() ?? new()
            };

            product.Options ??= new List<ProductOpt>();
            product.Options.Add(option);

            await _productRepository.UpdateAsync(product);

            return true;
        }

        public async Task<bool> UpdateOptionAsync(string sellerId, string storeId, string productId, string optionId, CreateProductOptRequest request)
        {
            var product = await GetOwnedProduct(sellerId, storeId, productId);

            if (product == null)
                return false;

            var option = product.Options?.FirstOrDefault(x => x.Id == optionId);

            if (option == null)
                return false;

            option.Name = request.Name;
            option.IsRequired = request.IsRequired;
            option.IsMultiple = request.IsMultiple;

            option.Values = request.Values?.Select(v => new ProductOptValue
            {
                Id = Guid.NewGuid().ToString(),
                Name = v.Name,
                PriceModifier = v.PriceModifier
            }).ToList() ?? new();

            await _productRepository.UpdateAsync(product);

            return true;
        }

        public async Task<bool> DeleteOptionAsync(string sellerId, string storeId, string productId, string optionId)
        {
            var product = await GetOwnedProduct(sellerId, storeId, productId);

            if (product == null)
                return false;

            var option = product.Options?.FirstOrDefault(x => x.Id == optionId);

            if (option == null)
                return false;

            product?.Options?.Remove(option);

            await _productRepository.UpdateAsync(product!);

            return true;
        }

        public async Task<List<ProductOptDto>> GetOptionsByProductIdAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return new();

            return product.Options?.Select(o => new ProductOptDto
            {
                Id = o.Id,
                Name = o.Name,
                IsRequired = o.IsRequired,
                IsMultiple = o.IsMultiple,
                CreatedAt = o.CreatedAt,
                Values = o.Values?.Select(v => new ProductOptValueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    PriceModifier = v.PriceModifier
                }).ToList() ?? new()
            }).ToList() ?? new();
        }

        // =========================
        // PRIVATE
        // =========================

        private static PagedProductResponse MapPaged(List<Product> products, GetProductQueryDto query)
        {
            var total = products.Count;

            var items = products
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    IsAvailable = x.IsAvailable,
                    ImageUrl = x.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                })
                .ToList();

            return new PagedProductResponse
            {
                Items = items,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        private static List<Product> ApplyCommonFilters(List<Product> products, GetProductQueryDto query)
        {
            return products
                .Where(x => !x.IsDeleted)
                .Where(x => string.IsNullOrWhiteSpace(query.CategoryId) || x.CategoryId == query.CategoryId)
                .Where(x => !query.MinPrice.HasValue || x.Price >= query.MinPrice.Value)
                .Where(x => !query.MaxPrice.HasValue || x.Price <= query.MaxPrice.Value)
                .Where(x => string.IsNullOrWhiteSpace(query.Search) || x.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}