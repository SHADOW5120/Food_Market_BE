namespace Food_Market_BE.Modules.ProductModule.DTOs.Media
{
    public class ProductImgDto
    {
        public string Id { get; set; } = default!;

        public string ImageUrl { get; set; } = default!;

        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}