using Food_Market_BE.Modules.CartModule.Models;

namespace Food_Market_BE.Modules.CartModule.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(string userId);
        Task<Cart> CreateAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(string cartId);
    }
}
