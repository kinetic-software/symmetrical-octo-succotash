using MongoDB.Driver;

namespace Kx.Core.Common.Data.MongoDB
{
    public interface IMongoDbConnection
    {        

        IMongoDatabase GetMongoDatabase();
    }
}
