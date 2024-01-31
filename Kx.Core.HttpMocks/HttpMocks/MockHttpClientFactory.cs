using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Kx.Core.HttpMocks.HttpMocks;

public class MockHttpClientFactory : IHttpClientFactory
{
    public IStubHost Host { get; private set; } = null!;
    private readonly Dictionary<string, HttpClient>? _clients = new();

    public MockHttpClientFactory(IConfiguration config)
    {
        config["LOCATIONS_URL"] = "localhost";
        config["BEDROOMS_URL"] = "localhost";
        config["CONFIG_URL"] = "localhost";
        config["DEFAULT_PAGE_SIZE"] = "1";
    }

    public HttpClient CreateClient(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return MakeTestClient();
        }

        HttpClient? client = null;
        if (_clients?.Count > 0)
        {
            client = _clients.FirstOrDefault(c => c.Key == name).Value;
        }

        if (client is not null) return client;
        
        client =  MakeTestClient();
        _clients?.Add(name, client);

        return client;
    }
    
    private HttpClient MakeTestClient()
    {
        var handler = new MockHttpMessageHandler();
        Host = handler.Host("http://localhost");
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = Host.Host,
            Timeout = Debugger.IsAttached ? Timeout.InfiniteTimeSpan : TimeSpan.FromMilliseconds(100.0)
        };
        return httpClient;
    }
}