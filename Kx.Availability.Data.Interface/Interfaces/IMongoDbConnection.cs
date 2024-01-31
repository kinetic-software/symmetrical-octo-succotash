using MongoDB.Driver;

namespace Kx.Availability.Data.Interface.Interfaces
{
    public interface IMongoDbConnection
    {        
        IMongoDatabase GetMongoDatabase();

        Task InitDb();
    }
}
