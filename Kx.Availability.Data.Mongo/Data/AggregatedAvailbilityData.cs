using System.Data;
using Kx.Availability.Data.Mongo.Models;
using Kx.Core.Common.Data;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace Kx.Availability.Data.Mongo.Data;

public class AggregatedAvailabilityData : IDataAccessAggregation
{
    private readonly ITenant _tenant;
    private readonly string _collectionName;
    private readonly IMongoCollection<DataLoadStateModel> _stateTableCollection;
    private readonly IMongoCollection<AvailabilityMongoModel> _tempAvailabilityCollection;
    private readonly IMongoCollection<AvailabilityMongoModel> _liveAvailabilityCollection;
    private readonly IMongoDatabase _database;
    private string _stateRecordIdForThisTenant;
    private readonly IConfiguration _config;
    private readonly double _timeoutHours;


    public AggregatedAvailabilityData(IConnectionDefinitionFactory connectionDefinitionFactory, ITenant tenant, IConfiguration config, IKxJsonSettings jsonSettings)
    {
        var mongoDbConnection = connectionDefinitionFactory.GetMongoDbConnection();
    
        _database = mongoDbConnection.GetMongoDatabase();
        _tenant = tenant;
        _config = config;

        _collectionName = $"{nameof(AvailabilityMongoModel)}{_tenant.TenantId}";
        _tempAvailabilityCollection = _database.GetCollection<AvailabilityMongoModel>(_collectionName);
        _liveAvailabilityCollection = _database.GetCollection<AvailabilityMongoModel>(nameof(AvailabilityMongoModel));
        _stateTableCollection = _database.GetCollection<DataLoadStateModel>(nameof(DataLoadStateModel));
        _stateRecordIdForThisTenant = string.Empty;
        // DC: Converted this into a private field; no need to have it continuously set as a variable inside the HasPreviousRunEnded() method, do it once in this constructor.
        _timeoutHours = Convert.ToDouble(_config.GetSection("DATA_LOAD_TIMEOUT_HOURS").Value ?? "0.1");
    }

    public void StartStateRecord()
    {
        var forTenantMessageExtension = $"for tenant {_tenant.TenantId}.";
        //DC: Felt the call to FetchRunStatus() could sit outside of the original try/catch body as we may be "fine"
        //    OR it may throw the DataException; if the later, there is no need to have that caught (again) by the more generic System.Exception catch.
        if (FetchRunStatus() != RunStatus.Ended) throw new DataException($"Cannot start a new run {forTenantMessageExtension} The previous run has not ended.");

        try
        {
            Log.Debug("Starting new run");

            AddIndexes();
            var startState = new DataLoadStateModel
            {
                TenantId = _tenant.TenantId,
                State = StateEventType.CycleStart.ToString(),
                StartTime = DateTime.UtcNow,
            };

            _stateTableCollection.InsertOne(startState);

            _stateRecordIdForThisTenant = startState.ID;
        }
        catch (Exception systemException)
        {
            Log.Error(systemException, $"Major exception occured when attempting to start new run {forTenantMessageExtension}");
            throw;
        }
    }

    private void AddIndexes()
    {
        /* Add any indexes that don't already exist. */
        var indexBuilder = Builders<AvailabilityMongoModel>.IndexKeys;
        List<CreateIndexModel<AvailabilityMongoModel>> indexesToCreate = new List<CreateIndexModel<AvailabilityMongoModel>>
        {
            new(indexBuilder.Ascending(x => x.TenantId)),
            new(indexBuilder.Ascending(x => x.TenantId).Ascending(x => x.RoomId)),            
            new(indexBuilder.Ascending(x => x.TenantId).Ascending("Locations._id").Ascending("Locations.Meta.ExternalId").Ascending("Locations.Meta.EntityVersion")),
            new(indexBuilder.Ascending(x => x.TenantId).Ascending("Locations.Type"))
        };

        _liveAvailabilityCollection.Indexes.CreateMany(indexesToCreate);
    }

    public RunStatus FetchRunStatus()
    {
        Log.Debug("Checking prev run");
        var stateRecord = 
            _stateTableCollection
                .AsQueryable()
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefault(s => s.TenantId == _tenant.TenantId);
        if (stateRecord is null)
        {
            return RunStatus.Unknown;
        }

        if (stateRecord.IsEnded) return RunStatus.Ended;

        if (stateRecord.StartTime <= DateTime.UtcNow.AddHours(-_timeoutHours))
        {
            return RunStatus.Ended;
        }

        return RunStatus.Running;
    }
    
    /// <summary>
    /// Deletes the temporary Collection(s) created
    /// </summary>
    public async Task DeleteAsync(IDataModel? data)
    {
        await _database.DropCollectionAsync(_collectionName);
    }
    
    public async Task InsertAsync(IDataModel? data)
    {               
        var aggAvailabilities = data as AggregatedAvailabilityModel;

        if (aggAvailabilities != null)
        {
            var availabilities = aggAvailabilities.Availability;
            await _tempAvailabilityCollection.InsertManyAsync(availabilities);
        }        
    }

    public async Task InsertStateAsync(ITenantDataModel stateRecord)
    {
        var record = stateRecord as DataLoadStateModel ?? new DataLoadStateModel();
        await _stateTableCollection.InsertOneAsync(record);
    }

    public async Task UpdateAsync()
    {
        var tempTenantAvailability = await _tempAvailabilityCollection.AsQueryable().ToListAsync();
        if (tempTenantAvailability.Any())
        {
            if (_liveAvailabilityCollection.AsQueryable().Any())
            {
                var deleteResult = await _liveAvailabilityCollection.DeleteManyAsync(
                    doc => doc.TenantId == _tenant.TenantId,
                    new DeleteOptions());

                if (deleteResult.IsAcknowledged)
                {
                    await InsertManyAvailabilityAsync(tempTenantAvailability);
                }
            }
            else
            {
                await InsertManyAvailabilityAsync(tempTenantAvailability);
            }
        }
        await UpdateStateAsync(StateEventType.CycleFinished, true);
    }

    private async Task InsertManyAvailabilityAsync(IEnumerable<AvailabilityMongoModel> tempTenantAvailability)
    {
        await _liveAvailabilityCollection.InsertManyAsync(tempTenantAvailability);
        await DeleteAsync(null);
    }
   
    public async Task UpdateStateAsync(
        StateEventType state,
        bool isCompleted = false, string? exception = null)
    {
        var stateRecord = await _stateTableCollection
            .AsQueryable()
            .FirstOrDefaultAsync(s => s.ID == _stateRecordIdForThisTenant);

        stateRecord.State = state.ToString();
        stateRecord.ExceptionMessage = exception;
        stateRecord.StateTime = DateTime.UtcNow;
        stateRecord.IsEnded = isCompleted;

        await _stateTableCollection.ReplaceOneAsync(
            doc => doc.ID == _stateRecordIdForThisTenant,
            stateRecord,
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task<int> CountAsync()
    {
        return await _liveAvailabilityCollection.AsQueryable().CountAsync();
    }
}