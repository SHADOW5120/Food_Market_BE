using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.ReviewModule.Models
{
    public class Rating
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = default!;

        public double AverageRating { get; set; }

        public int TotalReviews { get; set; }

        public int FiveStar { get; set; }

        public int FourStar { get; set; }

        public int ThreeStar { get; set; }

        public int TwoStar { get; set; }

        public int OneStar { get; set; }
    }
}
