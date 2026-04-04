using Food_Market_BE.Modules.ReviewModule.Models;
using Food_Market_BE.Modules.ReviewModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.ReviewModule.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IMongoCollection<Review> _reviewCollection;

        public ReviewRepository(MongoDbContext database)
        {
            _reviewCollection = database.GetCollection<Review>("Reviews");
        }

        public async Task<List<Review>> GetReviewsByProductAsync(string productId, int page, int pageSize, string sortBy)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.ProductId, productId);

            var sortDefinition = sortBy.ToLower() switch
            {
                "oldest" => Builders<Review>.Sort.Ascending(x => x.CreatedAt),
                "highest" => Builders<Review>.Sort.Descending(x => x.Rating).Descending(x => x.CreatedAt),
                "lowest" => Builders<Review>.Sort.Ascending(x => x.Rating).Descending(x => x.CreatedAt),
                _ => Builders<Review>.Sort.Descending(x => x.CreatedAt)
            };

            return await _reviewCollection
                .Find(filter)
                .Sort(sortDefinition)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountReviewsByProductAsync(string productId)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.ProductId, productId);
            return (int)await _reviewCollection.CountDocumentsAsync(filter);
        }

        public async Task<List<Review>> GetAllReviewsByProductAsync(string productId)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.ProductId, productId);
            return await _reviewCollection.Find(filter).ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(string reviewId)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Id, reviewId);
            return await _reviewCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Review?> GetUserReviewAsync(string userId, string productId)
        {
            var filter = Builders<Review>.Filter.And(
                Builders<Review>.Filter.Eq(x => x.UserId, userId),
                Builders<Review>.Filter.Eq(x => x.ProductId, productId)
            );

            return await _reviewCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateReviewAsync(Review review)
        {
            await _reviewCollection.InsertOneAsync(review);
        }

        public async Task UpdateReviewAsync(string reviewId, Review review)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Id, reviewId);
            await _reviewCollection.ReplaceOneAsync(filter, review);
        }

        public async Task DeleteReviewAsync(string reviewId)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Id, reviewId);
            await _reviewCollection.DeleteOneAsync(filter);
        }
    }
}
