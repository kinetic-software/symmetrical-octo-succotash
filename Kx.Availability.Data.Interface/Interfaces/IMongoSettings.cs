using MongoDB.Driver;

namespace Kx.Availability.Data.Interface.Interfaces
{
    public interface IMongoSettings
    {
        public string ConnectionString { get; }

        public IMongoClient MongoClient { get; }

        public string DatabaseName { get; }

    }
}
