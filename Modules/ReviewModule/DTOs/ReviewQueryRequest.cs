namespace Food_Market_BE.Modules.ReviewModule.DTOs
{
    public class ReviewQueryRequest
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        // newest | oldest | highest | lowest
        public string SortBy { get; set; } = "newest";
    }
}
