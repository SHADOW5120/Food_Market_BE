namespace Food_Market_BE.Modules.ProductModule.DTOs.Media
{
    public class CreateProductImgRequest
    {
        public string ImageUrl { get; set; } = default!;
        public bool IsPrimary { get; set; } = false;
    }
}
