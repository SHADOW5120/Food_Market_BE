namespace Food_Market_BE.Modules.StoreModule.DTOs
{
    public class UpdateStoreRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public bool? IsOpen { get; set; }
    }
}
