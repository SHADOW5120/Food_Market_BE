using MongoDB.Bson.Serialization.Attributes;

namespace Food_Market_BE.Modules.CartModule.Models
{
    public class CartItem
    {
        [BsonElement("productId")]
        public string ProductId { get; set; } = default!;

        [BsonElement("productName")]
        public string ProductName { get; set; } = default!;

        [BsonElement("productImage")]
        public string? ProductImage { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("subtotal")]
        public decimal Subtotal { get; set; }
    }
}
