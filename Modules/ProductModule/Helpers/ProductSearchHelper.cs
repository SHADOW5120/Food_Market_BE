using Food_Market_BE.Modules.ProductModule.DTOs.Product;
using Food_Market_BE.Modules.ProductModule.Models.Product;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.ProductModule.Helpers
{
    public static class ProductSearchHelper
    {
            public static FilterDefinition<Product> BuildFilter(GetProductQueryDto query)
            {
                var builder = Builders<Product>.Filter;
                var filters = new List<FilterDefinition<Product>>();

                // Get valid products (not deleted and available)
                filters.Add(builder.Eq(x => x.IsDeleted, false));
                filters.Add(builder.Eq(x => x.IsAvailable, true));

                // Category
                if (!string.IsNullOrWhiteSpace(query.CategoryId))
                {
                    filters.Add(builder.Eq(x => x.CategoryId, query.CategoryId));
                }

                // Price range
                if (query.MinPrice.HasValue)
                {
                    filters.Add(builder.Gte(x => x.Price, query.MinPrice.Value));
                }

                if (query.MaxPrice.HasValue)
                {
                    filters.Add(builder.Lte(x => x.Price, query.MaxPrice.Value));
                }

                // Search by name (regex)
                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    filters.Add(
                        builder.Regex(
                            x => x.Name,
                            new BsonRegularExpression(query.Search, "i")
                        )
                    );
                }

                return filters.Count > 0
                    ? builder.And(filters)
                    : builder.Empty;
            }

            public static SortDefinition<Product> BuildSort(GetProductQueryDto query)
            {
                return query.Sort?.ToLower() switch
                {
                    "priceasc" =>
                        Builders<Product>.Sort.Ascending(x => x.Price),

                    "pricedesc" =>
                        Builders<Product>.Sort.Descending(x => x.Price),

                    "oldest" =>
                        Builders<Product>.Sort.Ascending(x => x.CreatedAt),

                    "newest" or null =>
                        Builders<Product>.Sort.Descending(x => x.CreatedAt),

                    _ =>
                        Builders<Product>.Sort.Descending(x => x.CreatedAt)
                };
            }

            public static (int Skip, int Limit) BuildPaging(GetProductQueryDto query)
            {
                var page = query.Page <= 0 ? 1 : query.Page;
                var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

                return (
                    (page - 1) * pageSize,
                    pageSize
                );
            }
        }
    }
