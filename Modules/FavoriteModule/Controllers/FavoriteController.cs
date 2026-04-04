using Food_Market_BE.Modules.FavoriteModule.DTOs;
using Food_Market_BE.Modules.FavoriteModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.FavoriteModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            try
            {
                var userId = GetUserId();
                var favorites = await _favoriteService.GetUserFavoritesAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = "Favorites retrieved successfully.",
                    data = favorites
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
        {
            try
            {
                var userId = GetUserId();

                var result = await _favoriteService.AddToFavoritesAsync(userId, request.ProductId);

                return Ok(new
                {
                    success = true,
                    message = "Added to favorites successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(string productId)
        {
            try
            {
                var userId = GetUserId();

                var removed = await _favoriteService.RemoveFromFavoritesAsync(userId, productId);

                return Ok(new
                {
                    success = true,
                    message = removed ? "Removed from favorites successfully." : "Favorite not found.",
                    data = removed
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? throw new Exception("UserId not found in token.");
        }
    }
}
