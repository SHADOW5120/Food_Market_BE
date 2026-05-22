using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.StoreModule.Models;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.SellerModule.Helpers
{
    public static class SellerRepositoryHelper
    {
        public static async Task<List<string>> GetSellerStoreIdsAsync(
            IMongoCollection<Store> stores,
            string sellerId)
        {
            return await stores
                .Find(x => x.OwnerId == sellerId && !x.IsDeleted)
                .Project(x => x.Id)
                .ToListAsync();
        }

        public static async Task<List<string>> GetSellerProductIdsAsync(
            IMongoCollection<Store> stores,
            IMongoCollection<Product> products,
            string sellerId)
        {
            var storeIds = await GetSellerStoreIdsAsync(stores, sellerId);

            return await products
                .Find(x =>
                    storeIds.Contains(x.StoreId)
                    && !x.IsDeleted)
                .Project(x => x.Id)
                .ToListAsync();
        }
    }
}
