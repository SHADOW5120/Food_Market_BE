namespace Food_Market_BE.Modules.OrderModule.DTOs
{
    public class PagedOrderResponse
    {
        public List<OrderResponse> Items { get; set; } = new();

        public long TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
