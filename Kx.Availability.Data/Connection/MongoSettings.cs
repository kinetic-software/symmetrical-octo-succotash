using Kx.Core.Common.Data.MongoDB;
using MongoDB.Driver;

namespace Kx.Availability.Data.Connection
{
    public class MongoSettings : IMongoSettings
    {
        private string? _connectionString;
        private string? _databaseName;
        private IMongoClient? _mongoClient;

        public MongoClientSettings ClientSettings { get; init; }

        public MongoSettings()
        {
            _connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
            ClientSettings = MongoClientSettings.FromConnectionString(_connectionString);
        }

        public string ConnectionString
        {
            get
            {

                if (_connectionString == null)
                {
                    InitialiseConnection();
                }

                return _connectionString!;
            }
        }        

        public IMongoClient MongoClient
        {
            get 
            { 
                if(_mongoClient == null)
                {
                    InitialiseConnection();
                }

                return _mongoClient!; 
            }
        }

        public string DatabaseName
        {
            get
            {
                if (_databaseName == null)
                {
                    InitialiseConnection();
                }

                return _databaseName!;
            }
        }
       

        private void InitialiseConnection()
        {
            
            _connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
            _databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
            _mongoClient = new MongoClient(ClientSettings);
        }
    }
}
