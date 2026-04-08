using Food_Market_BE.Modules.ReviewModule.DTOs;
using Food_Market_BE.Modules.ReviewModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Food_Market_BE.Modules.ReviewModule.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("products/{id}/reviews")]
        public async Task<IActionResult> GetProductReviews(
            string id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "newest")
        {
            var query = new ReviewQueryRequest
            {
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy
            };

            var result = await _reviewService.GetProductReviewsAsync(id, query);
            return Ok(result);
        }

        [HttpGet("products/{id}/reviews/paged")]
        public async Task<IActionResult> GetPagedReviews(
            string id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "newest")
        {
            var query = new ReviewQueryRequest
            {
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy
            };

            var result = await _reviewService.GetPagedReviewsAsync(id, query);
            return Ok(result);
        }

        [HttpGet("products/{id}/rating-summary")]
        public async Task<IActionResult> GetRatingSummary(string id)
        {
            var result = await _reviewService.CalculateProductRatingAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("reviews")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
        {
            var userId = GetUserId();
            var result = await _reviewService.CreateReviewAsync(request, userId);

            return Ok(new
            {
                message = "Review created successfully.",
                data = result
            });
        }

        [Authorize]
        [HttpPut("reviews/{id}")]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewRequest request)
        {
            var userId = GetUserId();
            var result = await _reviewService.UpdateReviewAsync(id, request, userId);

            return Ok(new
            {
                message = "Review updated successfully.",
                data = result
            });
        }

        [Authorize]
        [HttpDelete("reviews/{id}")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            var userId = GetUserId();
            await _reviewService.DeleteReviewAsync(id, userId);

            return Ok(new
            {
                message = "Review deleted successfully."
            });
        }

        [Authorize]
        [HttpPost("reviews/{id}/helpful")]
        public async Task<IActionResult> MarkReviewHelpful(string id)
        {
            var userId = GetUserId();
            // Placeholder implementation
            return Ok(new
            {
                success = true,
                message = "Review marked as helpful."
            });
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value
                         ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("Unauthorized.");

            return userId;
        }
    }
}