using Kx.Availability.Data.Implementation;
using Kx.Availability.Data.Mongo.Data;
using Kx.Availability.Tests.Data;
using Kx.Core.Common.Data;
using Kx.Core.Common.Data.MongoDB;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Kx.Core.HttpMocks.HttpMocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SolidToken.SpecFlow.DependencyInjection;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Kx.Availability.Tests.Dependencies;

/// <summary>
/// This class manages the scenario dependencies that are required for running the test. These
/// can then be injected into the test steps.
/// </summary>
public class ScenarioDependencies
{
    /// <summary>
    /// Creates the dependencies for the tests. Each scenario will have these dependencies.
    /// </summary>
    /// <returns>A service collection that defines the classes that can be injected.</returns>
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddEnvironmentVariables();


        var services = builder.Services;

        /* Add the dependencies that you require for your tests here. */
        
        services.AddHttpContextAccessor();        
        services.AddScoped<IHttpClientFactory, MockHttpClientFactory>();
        services.AddScoped<ITenant, TestTenant>();
        services.AddScoped<IConnectionDefinitionFactory, TestConnectionDefinitionFactory>();
        services.AddScoped<IDataAccessFactory, DataAccessFactory>();
        services.AddScoped<ITestDataAccessFactory, TestMongoDataAccessFactory>();
        services.AddScoped<IMongoSettings, TestMongoSettings>();
        services.AddScoped<IDataAggregationService, DataAggregationService>();                
        services.AddSingleton<IKxJsonSettings, KxJsonTestSettings>();                        

        return services;
    }
}
