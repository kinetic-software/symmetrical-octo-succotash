using Kx.Core.Common.Interfaces;

namespace Kx.Availability.Tests.Data;

public interface ITestData
{
    Task InsertAsync(IDataModel item);
    Task<object?> GetAllItemsAsync(string tableName);
    Task DeleteTableAsync();
    Task DeleteStateTableAsync();
}
