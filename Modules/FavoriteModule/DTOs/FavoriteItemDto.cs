namespace Food_Market_BE.Modules.FavoriteModule.DTOs
{
    public class FavoriteItemDto
    {
        public string FavoriteId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
