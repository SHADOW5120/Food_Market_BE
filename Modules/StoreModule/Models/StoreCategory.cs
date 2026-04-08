using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.StoreModule.Models
{
    public class StoreCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string StoreId { get; set; } = default!;

        public string Name { get; set; } = default!;
    }
}
