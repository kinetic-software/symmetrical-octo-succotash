using Kx.Core.Common.Exceptions;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Serilog;

namespace Kx.Availability.Modules;

public static class DataAggregationModule
{
    public static void ConfigureDataAggregationsApi(this WebApplication app)
    {
        // ReSharper disable once RouteTemplates.RouteParameterIsNotPassedToMethod
        app.MapPost("/v1/{tenantId}/bedroom-availability/reloadData", ReloadOneTenantsData);
    }

    /// <summary>
    /// Reloads the data for a single tenant.
    /// </summary>
    /// <returns>A Http response</returns>
    private static async Task<IResult> ReloadOneTenantsData(IDataAggregationService dataService)
    {
        try
        {            
            var results = await dataService.ReloadOneTenantsDataAsync();
            return ReturnResults.Result(results);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed reload tenants data");
            return Results.Problem(ex.ToString());
        }
    }
}
