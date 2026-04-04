using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.ProductModule.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string SellerId { get; set; } = default!;

        public bool IsAvailable { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public int Popularity { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
