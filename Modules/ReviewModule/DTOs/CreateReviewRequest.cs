using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.ReviewModule.DTOs
{
    public class CreateReviewRequest
    {
        [Required]
        public string ProductId { get; set; } = default!;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public List<string> Images { get; set; } = new();
    }
}
