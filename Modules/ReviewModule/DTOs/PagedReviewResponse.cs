namespace Food_Market_BE.Modules.ReviewModule.DTOs
{
    public class PagedReviewResponse
    {
        public List<ReviewItemDto> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
