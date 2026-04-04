using Food_Market_BE.Modules.ReviewModule.Models;

namespace Food_Market_BE.Modules.ReviewModule.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetReviewsByProductAsync(string productId, int page, int pageSize, string sortBy);
        Task<int> CountReviewsByProductAsync(string productId);
        Task<List<Review>> GetAllReviewsByProductAsync(string productId);
        Task<Review?> GetReviewByIdAsync(string reviewId);
        Task<Review?> GetUserReviewAsync(string userId, string productId);
        Task CreateReviewAsync(Review review);
        Task UpdateReviewAsync(string reviewId, Review review);
        Task DeleteReviewAsync(string reviewId);
    }
}
