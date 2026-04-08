using MongoDB.Driver;

namespace Food_Market_BE.Shared.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            _db = client.GetDatabase(config["MongoDb:DatabaseName"]);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }   
    }
}
