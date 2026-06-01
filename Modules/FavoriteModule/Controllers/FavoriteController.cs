using Food_Market_BE.Modules.FavoriteModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.FavoriteModule.Controllers
{
    [ApiController]
    [Route("api/favorites")]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return Ok(new { success = true, data = favorites });
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddToFavoriteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = GetUserId();
            var result = await _favoriteService.AddToFavoritesAsync(userId, request.ProductId);
            return Ok(new { success = true, data = result });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(string productId)
        {
            var userId = GetUserId();
            var removed = await _favoriteService.RemoveFromFavoritesAsync(userId, productId);
            return Ok(new { success = true });
        }

        private string GetUserId()
        {
                 return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value
                     ?? throw new UnauthorizedAccessException("UserId not found in token.");
        }
    }

    public class AddToFavoriteRequest
    {
        public string ProductId { get; set; } = string.Empty;
    }
}