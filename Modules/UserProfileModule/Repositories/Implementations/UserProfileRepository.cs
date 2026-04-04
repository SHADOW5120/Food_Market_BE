using Food_Market_BE.Modules.UserProfileModule.Models;
using Food_Market_BE.Modules.UserProfileModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.UserProfileModule.Repositories.Implementations
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly IMongoCollection<UserProfile> _collection;

        public UserProfileRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<UserProfile>("UserProfiles");
        }

        public async Task<UserProfile> GetByUserIdAsync(string userId)
        {
            return await _collection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(UserProfile profile)
        {
            await _collection.InsertOneAsync(profile);
        }

        public async Task UpdateAsync(UserProfile profile)
        {
            await _collection.ReplaceOneAsync(x => x.Id == profile.Id, profile);
        }
    }
}
