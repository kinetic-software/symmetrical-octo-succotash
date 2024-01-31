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

    [Then(@"I call the locations Api and get the following locations data")]
    public void ThenICallTheLocationsApiAndGetTheFollowingLocationsData(string jsonData)
    {
        _locRequest?.Reply(HttpStatusCode.OK,  jsonData, "application/json");
    }
    
    [Given(@"I have set an HttpRequestHandler for Rooms")]
    public void GivenIHaveSetAnHttpRequestHandlerForRooms()
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
    
    [Then(@"I call the Bedrooms Api to get the following rooms data")]
    public void ThenICallTheBedroomsApiToGetTheFollowingRoomsData(string jsonData)
    {
        _roomRequest?.Reply(HttpStatusCode.OK, jsonData, "application/json");    
    }


    [Then(@"return true")]
    public void ThenReturnTrue()
    {
        true.Should().Be(true);
    }

    [Given(@"I have set an HttpRequestHandler for Locations")]
    public void GivenIHaveSetAnHttpRequestHandlerForLocations()
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
    public async Task WhenIRequestTenantsDataToBeLoadedIntoTheMongodb()
    {              
        var result = await _aggregationService.ReloadOneTenantsDataAsync();
        SharedSteps.ActualStatusCode = result.statusCode;
        SharedSteps.ErrorResult = result.result;
    }


    [Given(@"I have the following data in the state table")]
    public async Task GivenIHaveTheFollowingDataInTheStateTable(string jsonData)
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
    public void ThenIReceiveAExpectationFailedResponse()
    {
        SharedSteps.ActualStatusCode.Should().Be(HttpStatusCode.ExpectationFailed);
    }

    [Then(@"I see the following message:")]
    public void ThenISeeTheFollowingMessage(string multilineText)
    {
        SharedSteps.ErrorResult.Should().Be(multilineText);
    }

    [Given(@"I have the following data in the state table that started past the timeout")]
    public async Task GivenIHaveTheFollowingDataInTheStateTableThatStartedPastTheTimeout(string jsonData)
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