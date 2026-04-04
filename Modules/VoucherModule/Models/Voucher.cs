using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Food_Market_BE.Modules.VoucherModule.Models
{
    public class Voucher
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        public string Code { get; set; } = default!;
        public string? Description { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? DiscountAmount { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? DiscountPercent { get; set; }

        public DateTime ExpiryDate { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MinOrderAmount { get; set; } = 0;

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? MaxDiscountAmount { get; set; }

        public int UsedCount { get; set; } = 0;
        public int? MaxUsage { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
