using MongoDB.Driver;

namespace Kx.Core.Common.Data.MongoDB
{
    public interface IMongoSettings
    {
        public string ConnectionString { get; }

        public IMongoClient MongoClient { get; }

        public string DatabaseName { get; }
        
        public MongoClientSettings ClientSettings { get; init; }

    }
}
