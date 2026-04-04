namespace Food_Market_BE.Modules.ReviewModule.DTOs
{
    public class ProductReviewDto
    {
        public string ReviewId { get; set; } = default!;

        public string UserId { get; set; } = default!;

        public string Username { get; set; } = default!;

        public string? UserAvatar { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public List<string> Images { get; set; } = new();

        public DateTime CreatedDate { get; set; }
    }
}
