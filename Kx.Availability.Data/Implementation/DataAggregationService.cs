using System.Data;
using System.Net;
using System.Net.Http.Json;
using Kx.Availability.Data.Exceptions;
using Kx.Availability.Data.Mongo.Models;
using Kx.Availability.Data.Mongo.StoredModels;
using Kx.Core.Common.Data;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Serilog;
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

// ReSharper disable PossibleMultipleEnumeration

namespace Kx.Availability.Data.Implementation;

public class DataAggregationService : IDataAggregationService
{
    private readonly ITenant _tenant;
    private readonly IHttpClientFactory _httpClientFactory;        
    private readonly IDataAccessAggregation _aggregateData;    
    private readonly string? _coreBedroomsUrl;
    private readonly string? _coreLocationsUrl;
    private readonly IDataAggregationStoreAccess<LocationsDataStoreModel> _locationsData;
    private readonly IDataAggregationStoreAccess<BedroomsDataStoreModel> _roomsData;    
    private readonly int _pageSize;
    private readonly string? _mongoId;         
        

    public DataAggregationService(IDataAccessFactory dataAccessFactory, ITenant tenant, IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _tenant = tenant;
        _httpClientFactory = httpClientFactory;                        

        var dbAccessAggregate = dataAccessFactory.GetDataAccess(KxDataType.AvailabilityAggregation);
        _aggregateData = DataAccessHelper.ParseAggregationDataAccess(dbAccessAggregate);

        _locationsData = dataAccessFactory.GetDataStoreAccess<LocationsDataStoreModel>();
        _roomsData = dataAccessFactory.GetDataStoreAccess<BedroomsDataStoreModel>();
        
        
        _coreBedroomsUrl = config.GetSection("BEDROOMS_URL").Value;
        _coreLocationsUrl = config.GetSection("LOCATIONS_URL").Value;        
        _mongoId = config.GetSection("MongoID").Value ?? null;        
        
        //DC: Purely an observation, default of 1000 records (per page) feels wrong / dodgy (in the unlikely but possible scenario we havent configured on inside the configuration)
        _pageSize = 1000;
        if (int.TryParse(config.GetSection("DEFAULT_PAGE_SIZE").Value, out var pageSize))
        {
            _pageSize = pageSize;
        }        
    }

    private async Task CreateIndexes()
    {
        await CreateLocationsIndexes();
        await CreateRoomsIndexes();
    }

    private async Task CreateLocationsIndexes()
    {
        var indexBuilder = Builders<LocationsDataStoreModel>.IndexKeys;
        var indexModel = new CreateIndexModel<LocationsDataStoreModel>(indexBuilder
            .Ascending(x => x.ExternalId)
            .Ascending(x => x.Type)
            .Ascending(x => x.Id)
            .Ascending(p => p.ParentId));
        await _locationsData.AddIndex(indexModel);
    }

    private async Task CreateRoomsIndexes()
    {
        var indexBuilder = Builders<BedroomsDataStoreModel>.IndexKeys;
        var indexModel = new CreateIndexModel<BedroomsDataStoreModel>(indexBuilder.Ascending(x => x.RoomId));
        await _roomsData.AddIndex(indexModel);
    }  

    public async Task<(HttpStatusCode statusCode, string result)> ReloadOneTenantsDataAsync()
    {
        try
        {                        
            _aggregateData.StartStateRecord();
            
            Log.Information("Cleaning tmp table");
            await CleanTenantTempTablesAsync();
            
            await CreateIndexes();
                                    
            //1. Get Locations 
            var locationsTask = FetchLocationsAsync();
                                    
            //2. Get the rooms
            var roomsTask = FetchRoomsAsync();
                                            
            await Task.WhenAll(locationsTask, roomsTask);
            
            //3. Mash them together
            //make the main table from all imported tables            
            await MashTempTablesIntoTheAvailabilityModelAsync();
            
            //4. save tenantAvailabilityModel.            
            await MoveTempTenantToLive();                        
            await CleanTenantTempTablesAsync();
                        
            return (HttpStatusCode.NoContent, string.Empty);
            
        }
        catch (Exception ex)
        {
            return (HttpStatusCode.ExpectationFailed, ex.Message);
        }
    }

    private async Task MashTempTablesIntoTheAvailabilityModelAsync()
    {
        try
        {            
            var aggregatedAvailabilityModel = GetAggregatedDataStoreModel();

            var rooms = _roomsData.QueryFreely();

            if (rooms is null || !rooms.Any()) throw new DataException();                                   
            
            foreach (var room in rooms)
            {                                
                var availabilityModel = CreateAvailabilityMongoModel();
                availabilityModel.ID = _mongoId;
                if (_mongoId is null)
                {
                    availabilityModel.ID = Convert.ToString(availabilityModel.GenerateNewID());
                }

                availabilityModel.TenantId = _tenant.TenantId;
                availabilityModel.RoomId = room.RoomId;
                
                var addLocations = AddLocationModels(room);
                availabilityModel.Locations.AddRange(addLocations);
                
                aggregatedAvailabilityModel.Availability.Add(availabilityModel);
            }
          
            await _aggregateData.InsertAsync(aggregatedAvailabilityModel);
          
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to mash data together: {ex}");
            throw;
        }
    }    

    private AvailabilityMongoModel CreateAvailabilityMongoModel()
    {
        return new AvailabilityMongoModel
        {
            TenantId = _tenant.TenantId,
            RoomId = string.Empty, 
            Locations = new List<LocationModel>()
        };
    }
   
    private IEnumerable<LocationModel> AddLocationModels(BedroomsDataStoreModel room)
    {
        try
        {
            var locationsQuery = _locationsData.QueryFreely();

            /* Add the direct parent area */
            var tempLocations =
                locationsQuery?
                    .Where(l => l.Id == room.LocationID 
                                && (l.Type.ToLower() != "area" && l.Type.ToLower() != "site"))
                    .Select(loc => new LocationModel
                    {
                        Id = loc.Id,
                        Name = loc.Name,
                        ParentId = loc.ParentId,
                        IsDirectLocation = true
                    }).ToList();


            if (!(tempLocations?.Count > 0))
                return tempLocations as IEnumerable<LocationModel> ?? new List<LocationModel>();


            var currentTopLevelAreaIndex = 0;
            
            while (!tempLocations.Exists(x => x.ParentId == null))
            {
                var parentLocation = tempLocations[currentTopLevelAreaIndex].ParentId;

                var nextParentLocation =
                    locationsQuery?
                        .Where(l => l.Id == parentLocation)
                        .Select(loc => new LocationModel
                        {
                            Id = loc.Id,
                            Name = loc.Name,
                            ParentId = loc.ParentId,
                            IsDirectLocation = true
                        });

                if (nextParentLocation != null && nextParentLocation.Any())
                {
                    tempLocations.AddRange(nextParentLocation.ToList());
                    currentTopLevelAreaIndex++;
                }
                else
                {
                    Log.Error(
                        $"The location has a parent Id where the location does not exist ParentId: {parentLocation}");
                    break;
                }

                if (currentTopLevelAreaIndex >= tempLocations.Count) break;
                
            }


            return tempLocations as IEnumerable<LocationModel> ?? new List<LocationModel>();

        }
        catch (Exception ex)
        {
            Task.FromResult(async () => await LogStateErrorsAsync(LocationType.Locations, ex));
            throw;
        }
    }

    private AggregatedAvailabilityModel GetAggregatedDataStoreModel()
    {
        var data = new AggregatedAvailabilityModel
        {
            TenantId = _tenant.TenantId
        };
        return data;
    }  

    public async Task InsertStateAsync(ITenantDataModel item)
    {
        await _aggregateData.InsertStateAsync(item);
    }

    private async Task FetchLocationsAsync()
    {
        try
        {
            //DC: Dropped the passing of "1" to GetLocationsAsync() - Looks like it has a default parameter of 1 defined within itself
            var pageOfLocations = await GetLocationsAsync();         
            await _locationsData.InsertPageAsync<PaginatedStoreModel<LocationsDataStoreModel>>(pageOfLocations);

            if (pageOfLocations.TotalPages > 1)
            {
                for (var currentPage = 2; currentPage <= pageOfLocations.TotalPages; currentPage++)
                {
                    var page = await GetLocationsAsync(currentPage);
                    await _locationsData.InsertPageAsync<PaginatedStoreModel<LocationsDataStoreModel>>(page);
                }
            }
        }
        catch (Exception ex)
        {
            await LogStateErrorsAsync(LocationType.Locations, ex);
            throw;
        }
    }

    private async Task FetchRoomsAsync()
    {
        try
        {          
            var pageOfRooms = await GetRoomsFromBedroomsApiAsync();
            await _roomsData.InsertPageAsync<PaginatedStoreModel<BedroomsDataStoreModel>>(pageOfRooms);
         
            if (pageOfRooms.TotalPages > 1)
            {
                for (var currentPage = 2; currentPage <= pageOfRooms.TotalPages; currentPage++)
                {
         
                    var page = await GetRoomsFromBedroomsApiAsync(currentPage);
                    await _roomsData.InsertPageAsync<PaginatedStoreModel<BedroomsDataStoreModel>>(page);
                }         
            }
        }
        catch (Exception ex)
        {
            await LogStateErrorsAsync(LocationType.Rooms, ex);
            throw;
        }
    }

    private async Task<IPaginatedModel<BedroomsDataStoreModel>> GetRoomsFromBedroomsApiAsync(int pageNumber = 1)
    {       
        var uriBuilder = new UriBuilder(_coreBedroomsUrl!)
        {
            Path = $"production/v1/{_tenant.TenantId}/bedrooms/rooms",
            Query = $"pageSize={_pageSize}&page={pageNumber}"
        };
        var httpClient = _httpClientFactory.CreateClient(nameof(BedroomsDataStoreModel));

        return await GetDataFromApiAsync<BedroomsDataStoreModel>(uriBuilder, httpClient);
    }

    private async Task<IPaginatedModel<LocationsDataStoreModel>> GetLocationsAsync(int pageNumber = 1)
    {
        var uriBuilder = new UriBuilder(_coreLocationsUrl!)
        {
            Path = $"production/v1/{_tenant.TenantId}/locations",
            Query = $"pageSize={_pageSize}&page={pageNumber}"
        };
        var httpClient = _httpClientFactory.CreateClient(nameof(LocationsDataStoreModel));

        return await GetDataFromApiAsync<LocationsDataStoreModel>(uriBuilder, httpClient);
    }

    public async Task<IPaginatedModel<T>> GetDataFromApiAsync<T>(UriBuilder uriBuilder, HttpClient httpClient)
    {
        //DC: Wanted to pull this http client instantiation into this method given its all "generics" driven; so nameof type "T" will suffice; rather
        //    than having the line repeated for both the Bedrooms and Locations calls......BUT sadly it "upset" the mocked behaviour inside the Specflow Steps
        // var httpClient = _httpClientFactory.CreateClient(nameof(T));

        var response = await httpClient.GetAsync(uriBuilder.ToString());
        return await response.Content.ReadFromJsonAsync<PaginatedStoreModel<T>>() ??
               throw new UnprocessableEntityException();
    }

    private async Task MoveTempTenantToLive()
    {
        await _aggregateData.UpdateAsync();
    }

    private async Task CleanTenantTempTablesAsync()
    {
        await _locationsData.DeleteAsync();
        await _roomsData.DeleteAsync();        
    }

    private async Task LogStateErrorsAsync(LocationType changeTableType, Exception ex)
    {
        await LogStateErrorsAsync(changeTableType.ToString(), ex);
    }
    
    private async Task LogStateErrorsAsync(string changeType, Exception ex)
    {
        await _aggregateData.UpdateStateAsync(
            StateEventType.CycleError,
            true,
            ex.ToString());

        Log.Logger.Error(
            "Error inserting {S}{FullMessage}",
            changeType,
            ex.ToString());
    }
}
