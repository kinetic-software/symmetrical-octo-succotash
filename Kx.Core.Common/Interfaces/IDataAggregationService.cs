using System.Net;

namespace Kx.Core.Common.Interfaces;

public interface IDataAggregationService
{
    Task<(HttpStatusCode statusCode, string result)> ReloadOneTenantsDataAsync();
    Task InsertStateAsync(ITenantDataModel item);
}
