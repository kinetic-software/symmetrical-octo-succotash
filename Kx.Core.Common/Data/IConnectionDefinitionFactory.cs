using Kx.Core.Common.Data.MongoDB;

namespace Kx.Core.Common.Data;

public interface IConnectionDefinitionFactory
{    

    IMongoDbConnection GetMongoDbConnection();
}


