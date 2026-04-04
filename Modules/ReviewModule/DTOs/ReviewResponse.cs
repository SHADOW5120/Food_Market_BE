namespace Food_Market_BE.Modules.ReviewModule.DTOs
{
    public class ReviewResponse
    {
        public string ProductId { get; set; } = default!;

        public double AverageRating { get; set; }

        public int TotalReviews { get; set; }

        public RatingBreakdownDto Breakdown { get; set; } = new();

        public List<ReviewItemDto> ReviewList { get; set; } = new();
    }
}
