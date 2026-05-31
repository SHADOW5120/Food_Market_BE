using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Food_Market_BE.Modules.OrderModule.Models
{
    public class OrderItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = default!;

        public string ProductName { get; set; } = default!;

        public string? ProductImage { get; set; }

        // Giá gốc của món tại thời điểm order
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        // Danh sách option đã chọn (snapshot)
        public List<OrderItemOpt> Options { get; set; } = new();

        // (base + option) * quantity
        public decimal Subtotal { get; set; }
    }
}
