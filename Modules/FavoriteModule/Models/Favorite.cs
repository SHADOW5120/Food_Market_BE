using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.FavoriteModule.Models
{
    public class Favorite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = default!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
