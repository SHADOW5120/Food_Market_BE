using Food_Market_BE.Modules.ReviewModule.DTOs;

namespace Food_Market_BE.Modules.ReviewModule.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponse> GetProductReviewsAsync(string productId, ReviewQueryRequest query);
        Task<PagedReviewResponse> GetPagedReviewsAsync(string productId, ReviewQueryRequest query);
        Task<ReviewItemDto> CreateReviewAsync(CreateReviewRequest request, string userId);
        Task<ReviewItemDto> UpdateReviewAsync(string reviewId, UpdateReviewRequest request, string userId);
        Task<bool> DeleteReviewAsync(string reviewId, string userId);
        Task<ReviewResponse> CalculateProductRatingAsync(string productId);
    }
}
