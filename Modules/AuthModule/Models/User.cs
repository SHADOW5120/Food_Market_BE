using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public static class UserRole
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Seller = "Seller";
}

namespace Food_Market_BE.Modules.AuthModule.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}