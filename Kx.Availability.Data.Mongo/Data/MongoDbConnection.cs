using Kx.Core.Common.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Kx.Availability.Data.Mongo.Data
{
    public class MongoDbConnection : IMongoDbConnection
    {
        private readonly string _databaseName;        
        private readonly IMongoClient _mongoClient;
        private bool _dbInitialised;
        private readonly IMongoSettings _mongoSettings;
       

        public MongoDbConnection(IMongoSettings mongoSettings)
        {
            _databaseName = mongoSettings.DatabaseName;         
            _mongoClient = mongoSettings.MongoClient;
            _mongoSettings = mongoSettings;
        }  

        public IMongoDatabase GetMongoDatabase()
        {
            return _mongoClient.GetDatabase(_databaseName);
        }

        public async Task InitDb()
        {
            if (!_dbInitialised)
            {
                await DB.InitAsync(
                   _mongoSettings.DatabaseName,
                   _mongoSettings.ClientSettings
                );

                _dbInitialised = true;
            }
        }
    }
}
