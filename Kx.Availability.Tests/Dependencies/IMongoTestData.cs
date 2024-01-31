using Kx.Availability.Tests.Data;
using Kx.Core.Common.Interfaces;
using MongoDB.Entities;

namespace Kx.Availability.Tests.Dependencies;

public interface IMongoTestData : ITestData
{
    Task DeleteAllItemsAsync<T>() where T : class, ITenantDataModel, IEntity;
    Task InsertAsync<T>(T item) where T : ITenantDataModel, IEntity;
    Task<List<T>> GetAllItemsAsync<T>() where T : class, ITenantDataModel, IEntity;
}