using Food_Market_BE.Modules.OrderModule.Models;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using Food_Market_BE.Modules.SellerModule.DTOs.Dashboard;
using Food_Market_BE.Modules.SellerModule.Helpers;
using Food_Market_BE.Modules.SellerModule.Repositories.Interfaces;
using Food_Market_BE.Modules.StoreModule.Models;
using Food_Market_BE.Shared.Database;
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

        // ================= SUMMARY =================

        public async Task<SellerDashboardSummaryDto> GetDashboardSummaryAsync(string sellerId)
        {
            return new SellerDashboardSummaryDto
            {
                TotalCustomers = await CountCustomersAsync(sellerId),
                TotalOrders = await CountTotalOrdersAsync(sellerId),
                TotalProducts = await CountTotalProductsAsync(sellerId),
                TotalStores = await CountTotalStoresAsync(sellerId),
                TotalRevenue = await GetTotalRevenueAsync(sellerId)
            };
        }

        // ================= COUNTS =================

        public async Task<int> CountCustomersAsync(string sellerId)
        {
            var productIds = await SellerRepositoryHelper.GetSellerProductIdsAsync(_stores, _products, sellerId);

            var orders = await _orders.Find(x => x.Items.Any(i => productIds.Contains(i.ProductId))).ToListAsync();

            return orders
                .Select(x => x.UserId)
                .Distinct()
                .Count();
        }

        public async Task<int> CountTotalProductsAsync(string sellerId)
        {
            var storeIds = await SellerRepositoryHelper.GetSellerStoreIdsAsync(_stores, sellerId);

            return (int)await _products.CountDocumentsAsync(
                x => storeIds.Contains(x.StoreId)
                && !x.IsDeleted);
        }

        public async Task<int> CountTotalStoresAsync(string sellerId)
        {
            return (int)await _stores.CountDocumentsAsync(
                x => x.OwnerId == sellerId
                && !x.IsDeleted);
        }

        public async Task<int> CountTotalOrdersAsync(string sellerId)
        {
            var productIds = await SellerRepositoryHelper.GetSellerProductIdsAsync(_stores, _products, sellerId);

            return (int)await _orders.CountDocumentsAsync(
                x => x.Items.Any(i => productIds.Contains(i.ProductId)));
        }

        // ================= REVENUE =================

        public async Task<decimal> GetTotalRevenueAsync(string sellerId)
        {
            var productIds = await SellerRepositoryHelper.GetSellerProductIdsAsync(_stores, _products, sellerId);

            var orders = await _orders.Find(
                x => x.Status == OrderStatus.Completed
                && x.Items.Any(i => productIds.Contains(i.ProductId)))
                .ToListAsync();

            return orders.Sum(x => x.TotalPrice);
        }

        // ================= REVENUE CHARTS =================

        public async Task<List<TimeSeriesStatDto>> GetStoreRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            var productIds = await SellerRepositoryHelper.GetSellerProductIdsAsync(_stores, _products, sellerId);

            var orders = await _orders.Find(
                x => x.Status == OrderStatus.Completed
                && x.CreatedAt >= from
                && x.CreatedAt <= to
                && x.Items.Any(i => productIds.Contains(i.ProductId)))
                .ToListAsync();

            return orders
                .GroupBy(x => SellerRepositoryHelper.GetGroupTime(
                    x.CreatedAt,
                    groupBy))
                .Select(g => new TimeSeriesStatDto
                {
                    Time = g.Key,
                    Label = SellerRepositoryHelper.GetGroupLabel(
                        g.Key,
                        groupBy),
                    Value = g.Sum(x => x.TotalPrice)
                })
                .OrderBy(x => x.Time)
                .ToList();
        }

        public async Task<List<StoreRevenueSeriesDto>> GetRevenueByStoreTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            var stores = await _stores.Find(
                x => x.OwnerId == sellerId
                && !x.IsDeleted)
                .ToListAsync();

            var products = await _products.Find(x => !x.IsDeleted).ToListAsync();

            var orders = await _orders.Find(
                x => x.Status == OrderStatus.Completed
                && x.CreatedAt >= from
                && x.CreatedAt <= to)
                .ToListAsync();

            var result = new List<StoreRevenueSeriesDto>();

            foreach (var store in stores)
            {
                var storeProductIds = products
                    .Where(x => x.StoreId == store.Id)
                    .Select(x => x.Id)
                    .ToList();

                var filteredOrders = orders
                    .Where(x => x.Items.Any(i => storeProductIds.Contains(i.ProductId)))
                    .ToList();

                result.AddRange(
                    filteredOrders
                        .GroupBy(x => SellerRepositoryHelper.GetGroupTime(
                            x.CreatedAt,
                            groupBy))
                        .Select(g => new StoreRevenueSeriesDto
                        {
                            StoreId = store.Id,
                            StoreName = store.Name,

                            Time = g.Key,

                            Label = SellerRepositoryHelper.GetGroupLabel(
                                g.Key,
                                groupBy),

                            Revenue = g.Sum(x => x.TotalPrice)
                        })
                );
            }

            return result.OrderBy(x => x.Time).ToList();
        }

        public async Task<List<TimeSeriesStatDto>> GetProductRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            return await GetStoreRevenueTimeSeriesAsync(sellerId, from, to, groupBy);
        }

        public async Task<List<ProductRevenueSeriesDto>> GetRevenueByProductTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            var storeIds = await SellerRepositoryHelper.GetSellerStoreIdsAsync(_stores, sellerId);

            var products = await _products.Find(
                x => storeIds.Contains(x.StoreId)
                && !x.IsDeleted)
                .ToListAsync();

            var orders = await _orders.Find(
                x => x.Status == OrderStatus.Completed
                && x.CreatedAt >= from
                && x.CreatedAt <= to)
                .ToListAsync();

            var result = new List<ProductRevenueSeriesDto>();

            foreach (var product in products)
            {
                var relatedOrders = orders
                    .Where(x => x.Items.Any(i => i.ProductId == product.Id))
                    .ToList();

                result.AddRange(
                    relatedOrders
                        .GroupBy(x => SellerRepositoryHelper.GetGroupTime(
                        x.CreatedAt,
                        groupBy))
                    .Select(g => new ProductRevenueSeriesDto
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,

                        Time = g.Key,

                        Label = SellerRepositoryHelper.GetGroupLabel(
                            g.Key,
                            groupBy),

                        Revenue = g.Sum(x =>
                            x.Items
                             .Where(i => i.ProductId == product.Id)
                             .Sum(i => i.Subtotal))
                    })                                                             
                );
            }

            return result.OrderBy(x => x.Time).ToList();
        }

        public async Task<List<TimeSeriesStatDto>> GetOrderRevenueTimeSeriesAsync(string sellerId, DateTime from, DateTime to, string groupBy = "day")
        {
            return await GetStoreRevenueTimeSeriesAsync(sellerId, from, to, groupBy);
        }

        // ================= PIE CHART =================

        public async Task<List<StoreRevenuePieDto>> GetStoreRevenuePieStatsAsync(string sellerId)
        {
            var stores = await GetStoreStatsAsync(sellerId);

            var totalRevenue = stores.Sum(x => x.TotalRevenue);

            return stores.Select(x => new StoreRevenuePieDto
            {
                StoreId = x.StoreId,
                StoreName = x.StoreName,
                Revenue = x.TotalRevenue,
                Percentage = totalRevenue == 0
                    ? 0
                    : (double)(x.TotalRevenue / totalRevenue * 100)
            }).ToList();
        }

        public async Task<List<ProductRevenuePieDto>> GetProductRevenuePieStatsAsync(string sellerId)
        {
            var products = await GetAllProductRevenueStatsAsync(sellerId);

            var totalRevenue = products.Sum(x => x.TotalRevenue);

            return products.Select(x => new ProductRevenuePieDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Revenue = x.TotalRevenue,
                Percentage = totalRevenue == 0
                    ? 0
                    : (double)(x.TotalRevenue / totalRevenue * 100)
            }).ToList();
        }

        public async Task<List<StatusStatDto>> GetOrderStatusStatsAsync(string sellerId)
        {
            var productIds = await SellerRepositoryHelper.GetSellerProductIdsAsync(_stores, _products, sellerId);

            var orders = await _orders.Find(
                x => x.Items.Any(i => productIds.Contains(i.ProductId)))
                .ToListAsync();

            return orders
                .GroupBy(x => x.Status)
                .Select(g => new StatusStatDto
                {
                    Status = g.Key.ToString().ToLower(),
                    Count = g.Count()
                })
                .ToList();
        }

        // ================= STORE STATS =================

        public async Task<List<StoreStatsDto>> GetStoreStatsAsync(string sellerId)
        {
            var stores = await _stores.Find(
                x => x.OwnerId == sellerId
                && !x.IsDeleted)
                .ToListAsync();

            var products = await _products.Find(x => !x.IsDeleted).ToListAsync();

            var orders = await _orders.Find(x => true).ToListAsync();

            var result = new List<StoreStatsDto>();

            foreach (var store in stores)
            {
                var storeProducts = products.Where(x => x.StoreId == store.Id).ToList();

                var storeProductIds = storeProducts.Select(x => x.Id).ToList();

                var storeOrders = orders
                    .Where(x => x.Items.Any(i => storeProductIds.Contains(i.ProductId)))
                    .ToList();

                var revenue = storeOrders
                    .Where(x => x.Status == OrderStatus.Completed)
                    .Sum(x => x.TotalPrice);

                result.Add(new StoreStatsDto
                {
                    StoreId = store.Id,
                    StoreName = store.Name,
                    TotalProducts = storeProducts.Count,
                    TotalOrders = storeOrders.Count,
                    TotalRevenue = revenue,
                    AverageOrderValue = storeOrders.Count == 0 ? 0 : revenue / storeOrders.Count,
                    TotalCustomers = storeOrders.Select(x => x.UserId).Distinct().Count()
                });
            }

            return result;
        }

        public async Task<List<TopStoreRevenueDto>> GetTopRevenueStoresAsync(string sellerId, int top)
        {
            var stores = await GetStoreStatsAsync(sellerId);

            return stores
                .OrderByDescending(x => x.TotalRevenue)
                .Take(top)
                .Select(x => new TopStoreRevenueDto
                {
                    StoreId = x.StoreId,
                    StoreName = x.StoreName,
                    TotalRevenue = x.TotalRevenue,
                    TotalOrders = x.TotalOrders,
                    TotalProducts = x.TotalProducts
                })
                .ToList();
        }

        // ================= PRODUCT STATS =================

        public async Task<List<ProductStatsDto>> GetAllProductRevenueStatsAsync(string sellerId)
        {
            var storeIds = await SellerRepositoryHelper.GetSellerStoreIdsAsync(_stores, sellerId);

            var products = await _products.Find(
                x => storeIds.Contains(x.StoreId)
                && !x.IsDeleted)
                .ToListAsync();

            var stores = await _stores.Find(x => true).ToListAsync();

            var orders = await _orders.Find(x => true).ToListAsync();

            return products.Select(product =>
            {
                var productOrders = orders
                    .Where(x => x.Items.Any(i => i.ProductId == product.Id))
                    .ToList();

                return new ProductStatsDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    StoreId = product.StoreId,
                    StoreName = stores.FirstOrDefault(x => x.Id == product.StoreId)?.Name ?? "",
                    TotalOrders = productOrders.Count,
                    TotalSoldQuantity = productOrders.Sum(x => x.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity)),
                    TotalRevenue = productOrders.Sum(x => x.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Subtotal))
                };
            }).ToList();
        }

        public async Task<List<ProductStatsDto>> GetStoreProductRevenueStatsAsync(string storeId)
        {
            var sellerStoreProducts = await _products.Find(
                x => x.StoreId == storeId
                && !x.IsDeleted)
                .ToListAsync();

            var orders = await _orders.Find(x => true).ToListAsync();

            var store = await _stores.Find(x => x.Id == storeId).FirstOrDefaultAsync();

            return sellerStoreProducts.Select(product =>
            {
                var productOrders = orders
                    .Where(x => x.Items.Any(i => i.ProductId == product.Id))
                    .ToList();

                return new ProductStatsDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    StoreId = storeId,
                    StoreName = store?.Name ?? "",
                    TotalOrders = productOrders.Count,
                    TotalSoldQuantity = productOrders.Sum(x => x.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity)),
                    TotalRevenue = productOrders.Sum(x => x.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Subtotal))
                };
            }).ToList();
        }

        public async Task<List<TopProductDto>> GetTopRevenueProductsAsync(string sellerId, int top)
        {
            var products = await GetAllProductRevenueStatsAsync(sellerId);

            return products
                .OrderByDescending(x => x.TotalRevenue)
                .Take(top)
                .Select(x => new TopProductDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    StoreId = x.StoreId,
                    StoreName = x.StoreName,
                    TotalSoldQuantity = x.TotalSoldQuantity,
                    TotalRevenue = x.TotalRevenue
                })
                .ToList();
        }
    }
}