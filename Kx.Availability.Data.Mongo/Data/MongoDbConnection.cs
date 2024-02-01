using Kx.Core.Common.Data.MongoDB;
using MongoDB.Driver;

namespace Kx.Availability.Data.Mongo.Data
{
    public class MongoDbConnection : IMongoDbConnection
    {
        private readonly string _databaseName;        
        private readonly IMongoClient _mongoClient;
       

        public MongoDbConnection(IMongoSettings mongoSettings)
        {
            _databaseName = mongoSettings.DatabaseName;         
            _mongoClient = mongoSettings.MongoClient;
        }  

        public IMongoDatabase GetMongoDatabase()
        {
            return _mongoClient.GetDatabase(_databaseName);
        }
    }
}
