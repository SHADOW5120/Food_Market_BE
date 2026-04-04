using Food_Market_BE.Modules.CartModule.Models;
using Food_Market_BE.Modules.CartModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.CartModule.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Cart> _cartCollection;

        public CartRepository(MongoDbContext database)
        {
            _cartCollection = database.GetCollection<Cart>("Carts");
        }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _cartCollection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            await _cartCollection.InsertOneAsync(cart);
            return cart;
        }

        public async Task UpdateAsync(Cart cart)
        {
            await _cartCollection.ReplaceOneAsync(x => x.Id == cart.Id, cart);
        }

        public async Task DeleteAsync(string cartId)
        {
            await _cartCollection.DeleteOneAsync(x => x.Id == cartId);
        }
    }
}
