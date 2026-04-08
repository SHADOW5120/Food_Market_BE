namespace Food_Market_BE.Modules.StoreModule.DTOs
{
    public class StoreListItemDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? LogoUrl { get; set; }
        public double Rating { get; set; }
        public string? ShortDescription { get; set; }
        public int TotalProducts { get; set; }
        public bool IsOpen { get; set; }
    }
}
