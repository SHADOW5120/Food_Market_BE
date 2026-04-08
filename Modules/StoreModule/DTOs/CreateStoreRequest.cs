using System.ComponentModel.DataAnnotations;

namespace Food_Market_BE.Modules.StoreModule.DTOs
{
    public class CreateStoreRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
    }
}
