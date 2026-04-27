namespace Food_Market_BE.Modules.ProductModule.DTOs.Product
{
    public class GetProductsQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? Search { get; set; }
        public string? Sort { get; set; }
    }
}
