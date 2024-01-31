using Kx.Availability.Data.Interface.Enums;

namespace Kx.Availability.Data.Interface.Interfaces;

public interface IDataAggregationAccess : IDataAccess
{
    Task UpdateStateAsync(StateEventType state, string? entity, bool isSuccess = false, bool isCompleted = false, string? exception = null);
    Task InsertStateAsync(ITenantDataModel stateRecord);
    Task<int> CountAsync();
    IEnumerable<ITenantDataModel> QueryState();
    void StartStateRecord();
    IEnumerable<ITenantDataModel> QueryFreely();
    Task Delete();
    Task Update();
}