using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Food_Market_BE.Modules.OrderModule.Models
{
    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = default!;

        public string ProductName { get; set; } = default!;

        public string? ProductImage { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal { get; set; }
    }
}
