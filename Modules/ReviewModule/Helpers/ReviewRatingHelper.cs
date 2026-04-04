using Food_Market_BE.Modules.ReviewModule.DTOs;
using Food_Market_BE.Modules.ReviewModule.Models;

namespace Food_Market_BE.Modules.ReviewModule.Helpers
{
    public static class ReviewRatingHelper
    {
        public static Rating CalculateRating(string productId, List<Review> reviews)
        {
            if (reviews == null || !reviews.Any())
            {
                return new Rating
                {
                    ProductId = productId,
                    AverageRating = 0,
                    TotalReviews = 0,
                    FiveStar = 0,
                    FourStar = 0,
                    ThreeStar = 0,
                    TwoStar = 0,
                    OneStar = 0
                };
            }

            return new Rating
            {
                ProductId = productId,
                AverageRating = Math.Round(reviews.Average(x => x.Rating), 1),
                TotalReviews = reviews.Count,
                FiveStar = reviews.Count(x => x.Rating == 5),
                FourStar = reviews.Count(x => x.Rating == 4),
                ThreeStar = reviews.Count(x => x.Rating == 3),
                TwoStar = reviews.Count(x => x.Rating == 2),
                OneStar = reviews.Count(x => x.Rating == 1)
            };
        }

        public static RatingBreakdownDto ToBreakdownDto(Rating rating)
        {
            return new RatingBreakdownDto
            {
                FiveStar = rating.FiveStar,
                FourStar = rating.FourStar,
                ThreeStar = rating.ThreeStar,
                TwoStar = rating.TwoStar,
                OneStar = rating.OneStar
            };
        }
    }
}
