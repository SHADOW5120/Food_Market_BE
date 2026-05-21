using Food_Market_BE.Modules.CategoryModule.DTOs;
using Food_Market_BE.Modules.CategoryModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ProductModule.DTOs.Media;
using Food_Market_BE.Modules.ProductModule.DTOs.Options;
using Food_Market_BE.Modules.ProductModule.DTOs.Product;
using Food_Market_BE.Modules.ProductModule.Helpers;
using Food_Market_BE.Modules.ProductModule.Models.Media;
using Food_Market_BE.Modules.ProductModule.Models.Options;
using Food_Market_BE.Modules.ProductModule.Models.Product;
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

            products = products.Where(x => !x.IsDeleted && x.IsAvailable).ToList();

            if (!string.IsNullOrWhiteSpace(query.CategoryId))
                products = products.Where(x => x.CategoryId == query.CategoryId).ToList();

            if (query.MinPrice.HasValue)
                products = products.Where(x => x.Price >= query.MinPrice.Value).ToList();

            if (query.MaxPrice.HasValue)
                products = products.Where(x => x.Price <= query.MaxPrice.Value).ToList();

            if (!string.IsNullOrWhiteSpace(query.Search))
                products = products.Where(x => x.Name.ToLower().Contains(query.Search.ToLower())).ToList();

            var sorted = ProductSortHelper.ApplySort(products, query.Sort).ToList();

            var total = sorted.Count;

            var items = sorted
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

        public async Task<PagedProductResponse> GetSellerProductsAsync(string sellerId, GetProductsQueryDto query)
        {
            var products = await _productRepository.GetAllAsync();

            products = products.Where(x => x.StoreId == sellerId && !x.IsDeleted).ToList();

            if (!string.IsNullOrWhiteSpace(query.CategoryId))
                products = products.Where(x => x.CategoryId == query.CategoryId).ToList();

            if (query.MinPrice.HasValue)
                products = products.Where(x => x.Price >= query.MinPrice.Value).ToList();

            if (query.MaxPrice.HasValue)
                products = products.Where(x => x.Price <= query.MaxPrice.Value).ToList();

            if (!string.IsNullOrWhiteSpace(query.Search))
                products = products.Where(x => x.Name.ToLower().Contains(query.Search.ToLower())).ToList();

            var sorted = ProductSortHelper.ApplySort(products, query.Sort).ToList();

            var total = sorted.Count;

            var items = sorted
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

                IsAvailable = product.IsAvailable,
                CreatedAt = product.CreatedAt,

                Category = new CategoryDto
                {
                    Id = category?.Id ?? "",
                    Name = category?.Name ?? ""
                }
            };
        }

        public async Task<ProductDetailDto> CreateAsync(string storeId, CreateProductRequest request)
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

            return await GetByIdAsync(product.Id) ?? throw new Exception("Create failed");
        }

        public async Task<bool> UpdateAsync(string storeId, string productId, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            if (product.StoreId != storeId)
                return false;

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new Exception("Category not found");

            product.Name = request.Name.Trim();
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(string storeId, string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            if (product.StoreId != storeId)
                return false;

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> ToggleAvailabilityAsync(string storeId, string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            if (product.StoreId != storeId)
                return false;

            product.IsAvailable = !product.IsAvailable;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> AddOptionAsync(string productId, CreateProductOptRequest request)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            var option = new ProductOpt
            {
                Id = Guid.NewGuid().ToString(),
                FoodId = productId, // Assuming FoodId is ProductId
                Name = request.Name,
                IsRequired = request.IsRequired,
                IsMultiple = request.IsMultiple,
                CreatedAt = DateTime.UtcNow,
                Values = request.Values?.Select(v => new ProductOptValue
                {
                    Id = Guid.NewGuid().ToString(),
                    OptionId = Guid.NewGuid().ToString(), // This should be the option's Id, but since it's new, set after
                    Name = v.Name,
                    PriceModifier = v.PriceModifier,
                    CreatedAt = DateTime.UtcNow
                }).ToList() ?? new()
            };

            // Set the OptionId for values
            foreach (var value in option.Values)
            {
                value.OptionId = option.Id;
            }

            product.Options ??= new List<ProductOpt>();
            product.Options.Add(option);
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> UpdateOptionAsync(string productId, string optionId, CreateProductOptRequest request)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            var option = product.Options?.FirstOrDefault(o => o.Id == optionId);
            if (option == null)
                return false;

            option.Name = request.Name;
            option.IsRequired = request.IsRequired;
            option.IsMultiple = request.IsMultiple;

            // Update values - for simplicity, replace all
            option.Values = request.Values?.Select(v => new ProductOptValue
            {
                Id = Guid.NewGuid().ToString(),
                OptionId = optionId,
                Name = v.Name,
                PriceModifier = v.PriceModifier,
                CreatedAt = DateTime.UtcNow
            }).ToList() ?? new();

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteOptionAsync(string productId, string optionId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return false;

            var option = product.Options?.FirstOrDefault(o => o.Id == optionId);
            if (option == null)
                return false;

            product.Options?.Remove(option);
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<List<ProductOptDto>> GetOptionsByProductIdAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
                return new List<ProductOptDto>();

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
    }
}