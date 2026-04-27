using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Food_Market_BE.Modules.ProductModule.Models.Media;
using Food_Market_BE.Modules.ProductModule.Models.Options;

namespace Food_Market_BE.Modules.ProductModule.Models.Product
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public List<ProductImg> Images { get; set; } = new();

        public List<ProductOpt> Options { get; set; } = new();

        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string StoreId { get; set; } = default!;

        public bool IsAvailable { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public int Popularity { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
