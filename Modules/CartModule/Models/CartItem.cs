using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Food_Market_BE.Modules.CartModule.Models
{
    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = default!;

        public string ProductName { get; set; } = default!;

        public string? ProductImage { get; set; }

        // Giá gốc của món (không tính option)
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        // Danh sách option đã chọn
        public List<CartItemOpt> Options { get; set; } = new();

        // Tổng tiền của item (base + options) * quantity
        public decimal Subtotal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
