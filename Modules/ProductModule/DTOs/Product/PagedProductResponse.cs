namespace Food_Market_BE.Modules.ProductModule.DTOs.Product
{
    public class PagedProductResponse
    {
        public List<ProductDto> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
