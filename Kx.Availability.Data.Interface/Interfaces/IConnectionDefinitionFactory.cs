namespace Kx.Availability.Data.Interface.Interfaces;

public interface IConnectionDefinitionFactory
{
    IMongoDbConnection GetMongoDbConnection();

}