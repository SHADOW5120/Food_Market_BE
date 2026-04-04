using Food_Market_BE.Modules.CartModule.Dtos;
using Food_Market_BE.Modules.CartModule.Helpers;
using Food_Market_BE.Modules.CartModule.Models;
using Food_Market_BE.Modules.CartModule.Repositories.Interfaces;
using Food_Market_BE.Modules.CartModule.Services.Implementations;
using Food_Market_BE.Modules.ProductModule.Models;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.CartModule.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> GetCartAsync(string userId);
        Task<CartResponse> AddToCartAsync(string userId, AddToCartRequest request);
        Task<CartResponse> UpdateCartItemAsync(string userId, string productId, UpdateCartItemRequest request);
        Task<CartResponse> RemoveItemAsync(string userId, string productId);
        Task<CartResponse> ClearCartAsync(string userId);
        Task<string> CheckoutAsync(string userId, CheckoutRequest request);
    }
}
