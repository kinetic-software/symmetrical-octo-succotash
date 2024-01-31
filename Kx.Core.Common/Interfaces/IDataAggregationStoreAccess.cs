using MongoDB.Driver;

namespace Kx.Core.Common.Interfaces;

public interface IDataAggregationStoreAccess<T> where T:class
{
    Task InsertPageAsync<TU>(IPaginatedModel<T>? data) where TU: IPaginatedModel<T>;
    IQueryable<T>? QueryFreely();

    Task InsertAsync(T data);
    Task InsertManyAsync(IEnumerable<T> dataList);
    Task DeleteAsync();
    Task AddIndex(CreateIndexModel<T> indexModel);
}