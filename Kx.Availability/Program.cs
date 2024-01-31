using System.Text.Json.Serialization;
using Kx.Availability.Data.Connection;
using Kx.Availability.Data.Implementation;
using Kx.Availability.Data.Mongo.Data;
using Kx.Availability.Data.Mongo.Models;
using Kx.Availability.Data.Mongo.StoredModels;
using Kx.Availability.Modules;
using Kx.Core.Common.Data;
using Kx.Core.Common.Data.MongoDB;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using static System.Net.Mime.MediaTypeNames;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();                            
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
});

// Setting up Serilog for logging.
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
builder.Host.UseSerilog(
    (context, services, configuration) =>
        configuration.ReadFrom
            .Configuration(context.Configuration)
            .ReadFrom.Services(services)
            //.WriteTo.Console(new JsonFormatter())
            .Enrich.FromLogContext()
);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddScoped<ITenant, Tenant>();

builder.Services.AddSingleton<IKxJsonSettings, KxJsonSettings>();
builder.Services.AddScoped<IConnectionDefinitionFactory, ConnectionDefinitionFactory>();

builder.Services.AddScoped<IDataAccessFactory, DataAccessFactory>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMongoSettings, MongoSettings>();
builder.Services.AddScoped<IDataAggregationService, DataAggregationService>();

builder.Services.AddHttpClient(nameof(LocationsDataStoreModel), client => { 
});
builder.Services.AddHttpClient(nameof(BedroomsDataStoreModel), client => {
});

 
builder.Services.AddMemoryCache();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
});

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.

// Add swagger UI if we're in dev mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureDataAggregationsApi();

app.UseSerilogRequestLogging();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.ContentType = Text.Plain;

        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();


        await context.Response.WriteAsync(exceptionHandlerPathFeature?.Error.ToString() ?? "Unknown Exception");
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        Log.Logger.Error(exceptionHandlerPathFeature?.Error.ToString() ?? "Unknown Exception");

    });
});

/* Get the ASPNET_PORT environment variable so we can run the site on the correct port. If it's not
 * defined then we don't specify a port..
 */
var listenPort = Environment.GetEnvironmentVariable("ASPNET_PORT");

/* Temporary hack to allow us to read the body multiple times when loading the tenant
 * for process changes.
 */
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next.Invoke();    
});

if (string.IsNullOrWhiteSpace(listenPort))
{
    app.Run();
}
else
{
    app.Run($"http://*:{listenPort}");
}
