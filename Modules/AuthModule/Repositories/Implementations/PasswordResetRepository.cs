using Food_Market_BE.Modules.AuthModule.Models;
using Food_Market_BE.Modules.AuthModule.Repositories.Interfaces;
using Food_Market_BE.Shared.Database;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.AuthModule.Repositories.Implementations
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly IMongoCollection<PasswordResetToken> _collection;

        public PasswordResetRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<PasswordResetToken>("PasswordResetTokens");
        }

        public async Task CreateAsync(PasswordResetToken token)
        {
            await _collection.InsertOneAsync(token);
        }

        public async Task<PasswordResetToken> GetAsync(string token)
        {
            return await _collection.Find(x => x.Token == token).FirstOrDefaultAsync();
        }

        public async Task MarkUsedAsync(string token)
        {
            await _collection.UpdateOneAsync(
                x => x.Token == token,
                Builders<PasswordResetToken>.Update.Set(x => x.Used, true)
            );
        }
    }
}
