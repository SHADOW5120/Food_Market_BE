using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.ReviewModule.DTOs
{
    public class UpdateReviewRequest
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public List<string> Images { get; set; } = new();
    }
}
