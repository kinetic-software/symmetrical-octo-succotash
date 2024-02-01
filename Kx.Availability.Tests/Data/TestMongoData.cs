using Kx.Availability.Data.Mongo.Models;
using Kx.Core.Common.Interfaces;
using MongoDB.Entities;

namespace Kx.Availability.Tests.Data;

internal class TestMongoData : ITestData
{
    public async Task DeleteAllItemsAsync(IDataModel item)
    {
        if (item is AvailabilityMongoModel) await DB.DropCollectionAsync<AvailabilityMongoModel>();
        if (item is DataLoadStateModel) await DB.DropCollectionAsync<DataLoadStateModel>();
    }
    

    public async Task InsertAsync(IDataModel item)
    {
        switch (item)
        {
            case AvailabilityMongoModel availModel:
                await DB.SaveAsync(availModel);
                break;
            case DataLoadStateModel stateModel:
                await DB.SaveAsync(stateModel);
                break;    
        }
    }

    public async Task<object?> GetAllItemsAsync(string tableName)
    {
        return tableName switch
        {
            nameof(AvailabilityMongoModel) => await DB.Find<AvailabilityMongoModel>().ExecuteAsync(),            
            _ => null
        };
    }

    public async Task DeleteTableAsync()
    { 
        await DB.DropCollectionAsync<AvailabilityMongoModel>();        
    }

    public async Task DeleteStateTableAsync()
    {
        await DB.DropCollectionAsync<DataLoadStateModel>();
    }
}
