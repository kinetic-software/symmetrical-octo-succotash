
using Kx.Core.Common.Data.MongoDB;
using MongoDB.Driver;

namespace Kx.Availability.Tests.Data
{
    internal class TestMongoSettings : IMongoSettings
    {
        private string? _connectionString;
        public MongoClientSettings ClientSettings { get; init; } = null!;

        public string ConnectionString
        {
            get => _connectionString!;
            set
            {
                _connectionString = value;

                var clientSettings = MongoClientSettings
                    .FromConnectionString(_connectionString);

                MongoClient = new MongoClient(clientSettings);
            }
        }

        public IMongoClient MongoClient { get; private set; } = null!;
        public string DatabaseName => "TEST_DB_NAME";        
    }
}
