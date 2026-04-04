using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.ProductModule.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
