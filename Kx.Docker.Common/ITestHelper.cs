using Kx.Docker.Common.Shared;

namespace Kx.Docker.Common;

public interface ITestHelper
{
    Task<IConnectionDefinition> CreateDbPoolAsync(string dbName);
}

