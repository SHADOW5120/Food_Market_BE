using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.StoreModule.Models;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.SellerModule.Helpers
{
    public static class SellerRepositoryHelper
    {
        // ================= STORE =================

        public static async Task<List<string>> GetSellerStoreIdsAsync(
            IMongoCollection<Store> stores,
            string sellerId)
        {
            return await stores
                .Find(x => x.OwnerId == sellerId && !x.IsDeleted)
                .Project(x => x.Id)
                .ToListAsync();
        }

        public static async Task<List<Store>> GetSellerStoresAsync(
            IMongoCollection<Store> stores,
            string sellerId)
        {
            return await stores
                .Find(x => x.OwnerId == sellerId && !x.IsDeleted)
                .ToListAsync();
        }

        // ================= PRODUCT =================

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

        public static async Task<List<Product>> GetSellerProductsAsync(
            IMongoCollection<Store> stores,
            IMongoCollection<Product> products,
            string sellerId)
        {
            var storeIds = await GetSellerStoreIdsAsync(stores, sellerId);

            return await products
                .Find(x =>
                    storeIds.Contains(x.StoreId)
                    && !x.IsDeleted)
                .ToListAsync();
        }

        // ================= ORDER =================

        public static async Task<List<Order>> GetSellerOrdersAsync(
            IMongoCollection<Store> stores,
            IMongoCollection<Product> products,
            IMongoCollection<Order> orders,
            string sellerId)
        {
            var productIds = await GetSellerProductIdsAsync(
                stores,
                products,
                sellerId);

            return await orders
                .Find(x =>
                    x.Items.Any(i =>
                        productIds.Contains(i.ProductId)))
                .ToListAsync();
        }

        public static async Task<List<Order>> GetSellerCompletedOrdersAsync(
            IMongoCollection<Store> stores,
            IMongoCollection<Product> products,
            IMongoCollection<Order> orders,
            string sellerId)
        {
            var productIds = await GetSellerProductIdsAsync(
                stores,
                products,
                sellerId);

            return await orders
                .Find(x =>
                    x.Status == OrderStatus.Completed &&
                    x.Items.Any(i =>
                        productIds.Contains(i.ProductId)))
                .ToListAsync();
        }

        // ================= CHART =================

        public static string GetGroupLabel(
            DateTime date,
            string groupBy)
        {
            return groupBy.ToLower() switch
            {
                "hour" => date.ToString("yyyy-MM-dd HH:00"),
                "day" => date.ToString("yyyy-MM-dd"),
                "week" => $"{date.Year}-W{System.Globalization.ISOWeek.GetWeekOfYear(date)}",
                "month" => date.ToString("yyyy-MM"),
                "year" => date.ToString("yyyy"),
                _ => date.ToString("yyyy-MM-dd")
            };
        }

        public static DateTime GetGroupTime(
            DateTime date,
            string groupBy)
        {
            return groupBy.ToLower() switch
            {
                "day" => date.Date,

                "month" => new DateTime(
                    date.Year,
                    date.Month,
                    1),

                "year" => new DateTime(
                    date.Year,
                    1,
                    1),

                _ => date.Date
            };
        }
    }
}