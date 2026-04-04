using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.UserProfileModule.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string Username { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }

        public string Bio { get; set; } = "";
        public string Gender { get; set; } = "";
        public DateTime? Birthday { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
