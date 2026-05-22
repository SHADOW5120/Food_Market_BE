namespace Food_Market_BE.Modules.StoreModule.DTOs
{
    public class StoreResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? Description { get; set; }
        public double Rating { get; set; }
        public bool IsOpen { get; set; }

        public List<StoreCategoryDto> Categories { get; set; } = new();
        public List<StoreProductDto> Products { get; set; } = new();
    }
}
