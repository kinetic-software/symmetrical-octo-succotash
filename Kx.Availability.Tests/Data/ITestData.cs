using Kx.Core.Common.Enums;
using Kx.Core.Common.Interfaces;

namespace Kx.Availability.Tests.Data;

public interface ITestData
{
    Task DeleteAllItemsAsync(IDataModel item);
    Task InsertAsync(IDataModel item, ChangeTableType tableName);
    Task<object?> GetAllItemsAsync(string tableName);
    Task DeleteTableAsync();
    Task DeleteStateTableAsync();
}