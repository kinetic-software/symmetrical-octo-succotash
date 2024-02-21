using System.Diagnostics;
using System.Net;
using FluentAssertions;
using Kx.Availability.Data.Mongo.Models;
using Kx.Availability.Tests.Data;
using Kx.Availability.Tests.logging;
using Kx.Core.Common.Data;
using Kx.Core.Common.Data.MongoDB;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Kx.Core.HttpMocks.HttpMocks;
using Newtonsoft.Json;
using Serilog;
using TechTalk.SpecFlow;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming

namespace Kx.Availability.Tests.Specs;

[Binding]
public class SharedSteps : LoggedTestSteps
{
    public static string? Result;
    public static TestTenant Tenant = null!;
    private readonly IHttpClientFactory _httpClientFactory;
    public static ITestData? MongoTestData;
    public static Exception? _expectedException;
    public static string? ErrorResult { get; set; }
    public static HttpStatusCode ActualStatusCode;
    private readonly JsonSerializerSettings _jsonSettings;
    private readonly List<string> _tablesCreated = new();
    private readonly IMongoSettings _mongoSettings;
    private readonly ITestDataAccessFactory _mongoTestDataAccessFactory;
    private readonly IConnectionDefinitionFactory _mongoTestDataConnectionFactory;
    
    private IStubRequest? _locGetLocationIdsRequest;
    private IStubRequest? _locSearchLocationsRequest;    
                
    public SharedSteps(
        IKxJsonSettings jsonSettings,
        IMongoSettings mongoSettings,
        ITenant tenant,
        IConnectionDefinitionFactory mongoTestDataConnectionFactory,
        IHttpClientFactory httpClientFactory        
    ) : base(
        TestRunnerManager
            .GetTestRunner()
            .ScenarioContext.ScenarioContainer.Resolve<ITestOutputHelper>()
    )
    {
        _mongoSettings = mongoSettings;
        _mongoTestDataConnectionFactory = mongoTestDataConnectionFactory;
        _jsonSettings = jsonSettings.SerializerSettings;
        Tenant = (tenant as TestTenant)!;
        _mongoTestDataAccessFactory = new TestMongoDataAccessFactory();
        _httpClientFactory = httpClientFactory;                                     
    }


    [Given("I have a clean database")]
    public async Task SetupDb()
    {
        var name = _mongoSettings.DatabaseName;

        await ((TestConnectionDefinitionFactory)_mongoTestDataConnectionFactory).CreateTestMongoDatabase(name);

        /* Update the connection string environment variable here. */

        MongoTestData = _mongoTestDataAccessFactory.GetDataAccess();
        _expectedException = null;

        _tablesCreated.Add(name);
    }
     
    [Given(@"I have set an HttpRequestHandler for the Locations get location id endpoint with query ""(.*)"" that returns the following data:")]
    public void GivenIHaveSetAnHttpRequestHandlerForGetLocationIdsEndpoint(string query, string jsonData)
    {
        var uri = new UriBuilder
        {
            Path = MockRoutes.GetLocationIdsRoute(Tenant.TenantId),
            Query = $"{query}"
        };      

        _locGetLocationIdsRequest = (_httpClientFactory as MockHttpClientFactory)?
            .Host
            .Get(uri.ToString());        

        _locGetLocationIdsRequest?.Reply(HttpStatusCode.OK, jsonData, "application/json");
    }

    [Given(@"I have set an HttpRequestHandler for the Locations search endpoint with query ""(.*)"" that returns the following data:")]
    public void GivenIHaveSetAnHttpRequestHandlerForSearchLocationsEndpoint(string query, string jsonData)
    {
        var uri = new UriBuilder
        {
            Path = MockRoutes.GetLocationsRoute(Tenant.TenantId),
            Query = $"{query}"
        };

        _locSearchLocationsRequest = (_httpClientFactory as MockHttpClientFactory)?
            .Host
            .Get(uri.ToString());

        _locSearchLocationsRequest?.Reply(HttpStatusCode.OK, jsonData, "application/json");
    }
    
    [Given("I have the following availability in my database:")]
    public async Task InsertItemsIntoDB(string jsonData)
    {
        if (MongoTestData == null)
        {
            throw new NullReferenceException("MongoTestData is null");
        }

        try
        {
            var items = JsonConvert.DeserializeObject<List<AvailabilityMongoModel>>(jsonData, _jsonSettings);

            if (items != null)
            {
                foreach (var item in items)
                {
                    await MongoTestData.InsertAsync(item);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("DEBUG, failed to insert items, err: " + ex);
            throw;
        }
    }
           
    [Given(@"My tenant id is ""(.*)""")]
    public void SetTenantId(string tenantId)
    {
        Tenant.TenantId = tenantId;
    }          
        
    [Then("I receive a No Content response")]
    public void AssertResponseStatusCodeNoContent()
    {
        ActualStatusCode.Should().Be(HttpStatusCode.NoContent);
    }   

    [Then("I see the following availability in my database:")]
    public async Task AssertItemsInDB(string jsonData)
    {
        if (MongoTestData == null) { throw new NullReferenceException("MongoTestData is null"); }

        try
        {
            var expectedItems = JsonConvert.DeserializeObject<List<AvailabilityMongoModel>>(jsonData, _jsonSettings);

            var actualItems = await MongoTestData.GetAllItemsAsync(nameof(AvailabilityMongoModel));

            Assert.Equal(JsonConvert.SerializeObject(expectedItems, _jsonSettings), JsonConvert.SerializeObject(actualItems, _jsonSettings));
        }
        catch (Exception ex)
        {
            Console.WriteLine("DEBUG, failed to insert items, err: " + ex);
            throw;
        }
    }

    [AfterScenario("clearMongoTestData")]
    public async Task ClearMongoTestData()
    {
        try
        {
            Debug.Assert(MongoTestData != null, nameof(MongoTestData) + " != null");
            await MongoTestData.DeleteTableAsync();

            await MongoTestData.DeleteStateTableAsync()!;
        }
        catch (Exception ex)
        {
            Console.WriteLine("DEBUG, failed to delete items, err: " + ex);
            throw;
        }
    }

    [BeforeScenario("clearStateData")]
    public async Task ClearStateData()
    {
        try
        {
            var name = _mongoSettings.DatabaseName;

            await ((TestConnectionDefinitionFactory)_mongoTestDataConnectionFactory).CreateTestMongoDatabase(name);

            /* Update the connection string environment variable here. */

            MongoTestData = _mongoTestDataAccessFactory.GetDataAccess();

            _tablesCreated.Add(name);
            await MongoTestData.DeleteStateTableAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Information("DEBUG, failed to delete items, err: " + ex);
            throw;
        }
    }
}
