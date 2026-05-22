namespace Food_Market_BE.Modules.SellerModule.Repositories.Interfaces
{
    public interface ISellerRepository
    {
        // ================= DASHBOARD =================

        Task<int> CountTotalProductsAsync(string sellerId);

        Task<int> CountTotalOrdersAsync(string sellerId);

        Task<decimal> GetTotalRevenueAsync(string sellerId);

        Task<int> CountPendingOrdersAsync(string sellerId);
    }
}