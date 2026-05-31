namespace Food_Market_BE.Modules.StoreModule.DTOs
{
    public class GetStoreQueryDto
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
        public string? Sort { get; set; }

        public double? MinRating { get; set; }
        public double? MaxRating { get; set; }
    }
}
