using Food_Market_BE.Modules.ProductModule.Models.Product;

namespace Food_Market_BE.Modules.ProductModule.Helpers
{
    public static class ProductSortHelper
    {
        public static IEnumerable<Product> ApplySort(IEnumerable<Product> products, string? sort)
        {
            return sort?.ToLower() switch
            {
                "price_asc" => products.OrderBy(x => x.Price),
                "price_desc" => products.OrderByDescending(x => x.Price),
                "popularity" => products.OrderByDescending(x => x.Popularity),
                _ => products.OrderByDescending(x => x.CreatedAt)
            };
        }
    }
}
