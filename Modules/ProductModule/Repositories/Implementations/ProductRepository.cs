using Food_Market_BE.Modules.ProductModule.DTOs.Product;
using Food_Market_BE.Modules.ProductModule.Helpers;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.ProductModule.Repositories.Interfaces;
using Food_Market_BE.Modules.StoreModule.Models;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.ProductModule.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Store> _stores;

        public ProductRepository(MongoDbContext context)
        {
            _products = context.GetCollection<Product>("Products");
            _stores = context.GetCollection<Store>("Stores");
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _products.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Product product)
        {
            await _products.InsertOneAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            await _products.ReplaceOneAsync(x => x.Id == product.Id, product);
        }

        public async Task<List<Product>> GetByStoreIdAsync(string storeId)
        {
            return await _products.Find(x => x.StoreId == storeId).ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryIdAsync(string categoryId)
        {
            return await _products.Find(x => x.CategoryId == categoryId).ToListAsync();
        }

        public async Task<List<Product>> GetBySellerIdAsync(string sellerId)
        {
            var stores = await _stores
                .Find(x => x.OwnerId == sellerId)
                .ToListAsync();

            var storeIds = stores
                .Select(x => x.Id)
                .ToList();

            return await _products
                .Find(x => storeIds.Contains(x.StoreId))
                .ToListAsync();
        }

        public async Task<List<Product>> SearchAsync(GetProductQueryDto query)
        {
            var filter = ProductSearchHelper.BuildFilter(query);
            var sort = ProductSearchHelper.BuildSort(query);
            var paging = ProductSearchHelper.BuildPaging(query);

            return await _products
                .Find(filter)
                .Sort(sort)
                .Skip(paging.Skip)
                .Limit(paging.Limit)
                .ToListAsync();
        }
    }
}