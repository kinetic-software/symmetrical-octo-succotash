using System.Net;

namespace Kx.Availability.Data.Interface.Interfaces;

public interface IDataAggregationService
{
    Task<(HttpStatusCode statusCode, string result)> ReloadOneTenantsDataAsync();
    Task<int> CountAsync();
    Task<IPaginatedModel<T>> GetDataFromApiAsync<T>(UriBuilder uriBuilder, HttpClient httpClient);
    Task InsertStateAsync(ITenantDataModel stateRecord);
}