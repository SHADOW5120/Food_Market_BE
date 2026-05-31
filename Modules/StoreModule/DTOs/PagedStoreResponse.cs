namespace Food_Market_BE.Modules.StoreModule.DTOs
{
    public class PagedStoreResponse
    {
        public List<StoreResponse> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
