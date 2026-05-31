using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Preparing,
    Delivering,
    Completed,
    Cancelled
}

public class DeliveryAddress
{
    public string ReceiverName { get; set; }

    public string PhoneNumber { get; set; }

    public string AddressLine { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

namespace Food_Market_BE.Modules.OrderModule.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = default!;

        //[BsonRepresentation(BsonType.ObjectId)]
        //public string CartId { get; set; } = default!;

        public List<OrderItem> Items { get; set; } = new();

        public decimal Subtotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal TotalPrice { get; set; }

        [BsonRepresentation(BsonType.String)]
        public OrderStatus Status { get; set; }

        public DeliveryAddress DeliveryAddress { get; set; }

        public string PaymentMethod { get; set; } = default!;

        public string? VoucherCode { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }
    }
}
