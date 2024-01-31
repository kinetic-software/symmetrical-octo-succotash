using System.Net;

namespace Kx.Core.HttpMocks.HttpMocks;

public class MockHttpMessageHandler : DelegatingHandler
{
    private readonly Dictionary<string, StubHost> _servers = new ();
    public IReadOnlyCollection<IStubHost> Hosts => _servers.Values;

    public IStubHost Host(string? hostUri)
    {
        if (hostUri != null && _servers.TryGetValue(hostUri, out var stubHost)) return stubHost;

        if (hostUri == null) throw new HttpRequestException();
        
        stubHost = new StubHost(hostUri);
        _servers.Add(hostUri, stubHost);
        return stubHost;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {        
        if (!_servers.TryGetValue(request.RequestUri?.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped) ?? string.Empty, out var stubHost))
            throw new WebException("Error: NameResolutionFailure");
        
        if(!stubHost.IsPathRegistered(request))
        {
            throw new InvalidMockPathException(stubHost.RegisteredRequests(), request);
        }

        var httpResponseMessage = await stubHost.CreateResponseAsync(request, cancellationToken);
        httpResponseMessage.RequestMessage = request;
        
        return httpResponseMessage;
    }
}