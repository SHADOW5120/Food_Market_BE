using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.SellerModule.Helpers;
using Food_Market_BE.Modules.SellerModule.Repositories.Interfaces;
using Food_Market_BE.Modules.StoreModule.Models;
using Food_Market_BE.Shared.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.SellerModule.Repositories.Implementations
{
    public class SellerRepository : ISellerRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Store> _stores;

        public SellerRepository(MongoDbContext database)
        {
            _products = database.GetCollection<Product>("Products");
            _orders = database.GetCollection<Order>("Orders");
            _stores = database.GetCollection<Store>("Stores");
        }


        // ================= DASHBOARD =================

        public async Task<int> CountTotalProductsAsync(string sellerId)
        {
            var storeIds = await SellerRepositoryHelper
                .GetSellerStoreIdsAsync(_stores, sellerId);

            return (int)await _products.CountDocumentsAsync(
                x => storeIds.Contains(x.StoreId)
                && !x.IsDeleted);
        }

        public async Task<int> CountTotalOrdersAsync(string sellerId)
        {
            var productIds = await SellerRepositoryHelper
                .GetSellerProductIdsAsync(
                    _stores,
                    _products,
                    sellerId);

            return (int)await _orders.CountDocumentsAsync(
                x => x.Items.Any(i => productIds.Contains(i.ProductId)));
        }

        public async Task<decimal> GetTotalRevenueAsync(string sellerId)
        {
            var productIds = await SellerRepositoryHelper
                .GetSellerProductIdsAsync(
                    _stores,
                    _products,
                    sellerId);

            var orders = await _orders.Find(
                x => x.Status == "Completed"
                && x.Items.Any(i => productIds.Contains(i.ProductId)))
                .ToListAsync();

            return orders.Sum(x => x.TotalPrice);
        }

        public async Task<int> CountPendingOrdersAsync(string sellerId)
        {
            var productIds = await SellerRepositoryHelper
                .GetSellerProductIdsAsync(
                    _stores,
                    _products,
                    sellerId);

            return (int)await _orders.CountDocumentsAsync(
                x => x.Status == "Pending"
                && x.Items.Any(i => productIds.Contains(i.ProductId)));
        }
    }
}