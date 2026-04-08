using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.StoreModule.Models
{
    public class Store
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = default!;

        public double Rating { get; set; } = 0;

        public bool IsDeleted { get; set; } = false;

        public bool IsOpen { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
