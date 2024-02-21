using System.Net;
using FluentAssertions;
using Kx.Availability.Data.Mongo.Models;
using Kx.Availability.Data.Mongo.StoredModels;
using Kx.Availability.Tests.logging;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Kx.Core.HttpMocks.HttpMocks;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace Kx.Availability.Tests.Specs;

[Binding]
public class AggregateAvailabilityDataSteps : LoggedTestSteps
{
    private readonly IHttpClientFactory _httpClientFactory;
   
    private readonly IDataAggregationService _aggregationService;    
    
    private readonly JsonSerializerSettings _jsonSettings;
    private IStubRequest? _locRequest;
    private IStubRequest? _roomRequest;
        
    public AggregateAvailabilityDataSteps(
        IHttpClientFactory httpClientFactory,        
        IDataAggregationService aggregationService,        
        IKxJsonSettings jsonSettings) : base(
        TestRunnerManager
            .GetTestRunner()
            .ScenarioContext.ScenarioContainer.Resolve<ITestOutputHelper>()
    )
    {
        _jsonSettings = jsonSettings.SerializerSettings;
        
        _aggregationService = aggregationService;

        _httpClientFactory = httpClientFactory;        
    }

    [Given(@"The Locations API returns the following:")]
    public void CallLocationsApi(string jsonData)
    {
        _locRequest?.Reply(HttpStatusCode.OK,  jsonData, "application/json");
    }
    
    [Given(@"I have set an HttpRequestHandler for Rooms")]
    public void SetupBedroomsApi()
    {
        var uri = new UriBuilder 
        { 
            Path = $"/production/v1/{SharedSteps.Tenant.TenantId}/bedrooms/rooms",
            Query = $"pageSize={1}&page={1}"
        };

        _httpClientFactory.CreateClient(nameof(BedroomsDataStoreModel));
        
        _roomRequest = (_httpClientFactory as MockHttpClientFactory)?
            .Host
            .Get(uri.ToString());
    }
    
    [Given(@"The Bedrooms API returns the following:")]
    public void CallBedroomsApi(string jsonData)
    {
        _roomRequest?.Reply(HttpStatusCode.OK, jsonData, "application/json");    
    }


    [Then(@"return true")]
    public void ThenReturnTrue()
    {
        // DC: This is clearly wrong; asserting true is true - gives us no benefit here + it's not called from the feature.
        true.Should().Be(true);
    }

    [Given(@"I have set an HttpRequestHandler for Locations")]
    public void SetupLocationsApi()
    {
        var uri = new UriBuilder
        {
            Path = MockRoutes.GetLocationsRoute(SharedSteps.Tenant.TenantId),
            Query = $"pageSize={1}&page={1}"

        };
        
        _httpClientFactory.CreateClient(nameof(LocationsDataStoreModel));

        _locRequest = (_httpClientFactory as MockHttpClientFactory)?
            .Host
            .Get(uri.ToString());
    }

    [When(@"I request Tenants Data to be loaded into the Mongodb")]
    public async Task MakeRequest()
    {              
        var result = await _aggregationService.ReloadOneTenantsDataAsync();
        SharedSteps.ActualStatusCode = result.statusCode;
        SharedSteps.ErrorResult = result.result;
    }

    [Given(@"I have the following data in the state table")]
    public async Task PreloadStateTable(string jsonData)
    {
        if (SharedSteps.MongoTestData == null)
        {
            throw new NullReferenceException("testData is null");
        }

        try
        {
            var items = JsonConvert.DeserializeObject<List<DataLoadStateModel>>(jsonData, _jsonSettings);
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.StartTime = DateTime.UtcNow.AddHours(-0.05);
                    await _aggregationService.InsertStateAsync(item);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("DEBUG, test data is null " + ex);
            throw;
        }
    }

    [Then(@"I receive a ExpectationFailed response")]
    public void AssertExpectationFailedResponse()
    {
        SharedSteps.ActualStatusCode.Should().Be(HttpStatusCode.ExpectationFailed);
    }

    [Then(@"I see the following message:")]
    public void AssertResponse(string multilineText)
    {
        SharedSteps.ErrorResult.Should().Be(multilineText);
    }

    [Given(@"I have the following data in the state table that started past the timeout")]
    public async Task PreloadOldStateData(string jsonData)
    {
        
        if (SharedSteps.MongoTestData == null)
        {
            throw new NullReferenceException("testData is null");
        }

        try
        {
            var items = JsonConvert.DeserializeObject<List<DataLoadStateModel>>(jsonData, _jsonSettings);
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.StartTime = DateTime.UtcNow.AddHours(-3);
                    await _aggregationService.InsertStateAsync(item);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("DEBUG, test data is null " + ex);
            throw;
        }
    }
}
