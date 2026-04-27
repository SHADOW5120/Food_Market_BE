using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.ProductModule.Models.Media
{
    public class ProductImg
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string FoodId { get; set; } = default!;

        [BsonElement("image_url")]
        public string ImageUrl { get; set; } = default!;

        [BsonElement("is_primary")]
        public bool IsPrimary { get; set; } = false;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
