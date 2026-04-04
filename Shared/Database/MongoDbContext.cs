using Food_Market_BE.Modules.AuthModule.Models;
using Food_Market_BE.Modules.ProductModule.Models;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            _db = client.GetDatabase(config["MongoDB:Database"]);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }   
    }
}
