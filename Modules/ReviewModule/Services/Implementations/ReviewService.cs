using Food_Market_BE.Modules.ReviewModule.DTOs;
using Food_Market_BE.Modules.ReviewModule.Helpers;
using Food_Market_BE.Modules.ReviewModule.Models;
using Food_Market_BE.Modules.ReviewModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ReviewModule.Services.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.ReviewModule.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly MongoDbContext _database;

        public ReviewService(IReviewRepository reviewRepository, MongoDbContext database)
        {
            _reviewRepository = reviewRepository;
            _database = database;
        }

        public async Task<ReviewResponse> GetProductReviewsAsync(string productId, ReviewQueryRequest query)
        {
            var reviews = await _reviewRepository.GetReviewsByProductAsync(productId, query.Page, query.PageSize, query.SortBy);
            var allReviews = await _reviewRepository.GetAllReviewsByProductAsync(productId);

            var ratingSummary = ReviewRatingHelper.CalculateRating(productId, allReviews);

            var reviewDtos = new List<ReviewItemDto>();
            foreach (var review in reviews)
            {
                reviewDtos.Add(await MapToReviewItemDtoAsync(review));
            }

            return new ReviewResponse
            {
                ProductId = productId,
                AverageRating = ratingSummary.AverageRating,
                TotalReviews = ratingSummary.TotalReviews,
                Breakdown = ReviewRatingHelper.ToBreakdownDto(ratingSummary),
                ReviewList = reviewDtos
            };
        }

        public async Task<PagedReviewResponse> GetPagedReviewsAsync(string productId, ReviewQueryRequest query)
        {
            var reviews = await _reviewRepository.GetReviewsByProductAsync(productId, query.Page, query.PageSize, query.SortBy);
            var totalCount = await _reviewRepository.CountReviewsByProductAsync(productId);

            var items = new List<ReviewItemDto>();
            foreach (var review in reviews)
            {
                items.Add(await MapToReviewItemDtoAsync(review));
            }

            return new PagedReviewResponse
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ReviewItemDto> CreateReviewAsync(CreateReviewRequest request, string userId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.ProductId))
                throw new ArgumentException("ProductId is required.", nameof(request.ProductId));

            if (request.Rating < 1 || request.Rating > 5)
                throw new ArgumentOutOfRangeException(nameof(request.Rating), "Rating must be between 1 and 5.");

            var existedReview = await _reviewRepository.GetUserReviewAsync(userId, request.ProductId);
            if (existedReview != null)
                throw new InvalidOperationException("You have already reviewed this product.");

            var productExists = await CheckProductExistsAsync(request.ProductId);
            if (!productExists)
                throw new InvalidOperationException("Product not found.");

            var hasPurchased = await HasUserPurchasedProductAsync(userId, request.ProductId);
            if (!hasPurchased)
                throw new InvalidOperationException("You can only review products you have purchased.");

            var review = new Review
            {
                ProductId = request.ProductId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                Images = request.Images ?? new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.CreateReviewAsync(review);

            return await MapToReviewItemDtoAsync(review);
        }

        public async Task<ReviewItemDto> UpdateReviewAsync(string reviewId, UpdateReviewRequest request, string userId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to update this review.");

            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.Rating < 1 || request.Rating > 5)
                throw new ArgumentOutOfRangeException(nameof(request.Rating), "Rating must be between 1 and 5.");

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.Images = request.Images ?? new List<string>();
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateReviewAsync(reviewId, review);

            return await MapToReviewItemDtoAsync(review);
        }

        public async Task<bool> DeleteReviewAsync(string reviewId, string userId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to delete this review.");

            await _reviewRepository.DeleteReviewAsync(reviewId);
            return true;
        }

        public async Task<ReviewResponse> CalculateProductRatingAsync(string productId)
        {
            var allReviews = await _reviewRepository.GetAllReviewsByProductAsync(productId);
            var ratingSummary = ReviewRatingHelper.CalculateRating(productId, allReviews);

            return new ReviewResponse
            {
                ProductId = productId,
                AverageRating = ratingSummary.AverageRating,
                TotalReviews = ratingSummary.TotalReviews,
                Breakdown = ReviewRatingHelper.ToBreakdownDto(ratingSummary),
                ReviewList = new List<ReviewItemDto>()
            };
        }

        private async Task<ReviewItemDto> MapToReviewItemDtoAsync(Review review)
        {
            var (username, avatar) = await GetUserInfoAsync(review.UserId);

            return new ReviewItemDto
            {
                ReviewId = review.Id,
                UserId = review.UserId,
                Username = username,
                UserAvatar = avatar,
                Rating = review.Rating,
                Comment = review.Comment,
                Images = review.Images ?? new List<string>(),
                CreatedDate = review.CreatedAt
            };
        }

        private async Task<(string username, string? avatar)> GetUserInfoAsync(string userId)
        {
            try
            {
                var userProfiles = _database.GetCollection<BsonDocument>("UserProfiles");
                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(userId));
                var user = await userProfiles.Find(filter).FirstOrDefaultAsync();

                if (user == null)
                    return ("Unknown User", null);

                var username = user.Contains("Username") ? user["Username"].AsString :
                               user.Contains("FullName") ? user["FullName"].AsString :
                               user.Contains("Name") ? user["Name"].AsString :
                               "Unknown User";

                var avatar = user.Contains("AvatarUrl") ? user["AvatarUrl"].AsString :
                             user.Contains("Avatar") ? user["Avatar"].AsString :
                             null;

                return (username, avatar);
            }
            catch
            {
                return ("Unknown User", null);
            }
        }

        private async Task<bool> CheckProductExistsAsync(string productId)
        {
            try
            {
                var products = _database.GetCollection<BsonDocument>("Products");
                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(productId));
                var product = await products.Find(filter).FirstOrDefaultAsync();
                return product != null;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> HasUserPurchasedProductAsync(string userId, string productId)
        {
            try
            {
                var orders = _database.GetCollection<BsonDocument>("Orders");

                var filter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
                var userOrders = await orders.Find(filter).ToListAsync();

                foreach (var order in userOrders)
                {
                    if (!order.Contains("Items")) continue;

                    var items = order["Items"].AsBsonArray;
                    foreach (var item in items)
                    {
                        var itemDoc = item.AsBsonDocument;

                        if (itemDoc.Contains("ProductId") && itemDoc["ProductId"].AsString == productId)
                            return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
