using Food_Market_BE.Modules.AuthModule.Models;
using Food_Market_BE.Modules.AuthModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.AuthModule.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IMongoCollection<RefreshToken> _collection;

        public RefreshTokenRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<RefreshToken>("RefreshTokens");
        }

        public async Task CreateAsync(RefreshToken token)
        {
            await _collection.InsertOneAsync(token);
        }

        public async Task<RefreshToken> GetAsync(string token)
        {
            return await _collection.Find(x => x.Token == token).FirstOrDefaultAsync();
        }

        public async Task RevokeAsync(string token)
        {
            await _collection.UpdateOneAsync(
                x => x.Token == token,
                Builders<RefreshToken>.Update.Set(x => x.IsRevoked, true)
            );
        }
    }
}
