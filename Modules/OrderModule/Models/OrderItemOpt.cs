using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.OrderModule.Models
{
    public class OrderItemOpt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string OptionId { get; set; } = default!;

        public string OptionName { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ValueId { get; set; } = default!;

        public string ValueName { get; set; } = default!;

        public decimal PriceModifier { get; set; } = 0;
    }
}
