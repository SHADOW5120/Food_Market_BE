using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.CartModule.Models
{
    public class CartItemOpt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string CartItemId { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string OptionId { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ValueId { get; set; } = default!;

        public string? OptionName { get; set; }

        public string? ValueName { get; set; }

        public decimal PriceModifier { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
