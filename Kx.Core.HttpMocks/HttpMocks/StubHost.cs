namespace Kx.Core.HttpMocks.HttpMocks;

public class StubHost : IStubHost
{
    public Uri Host { get; }

    public List<StubRequest> StubRequests { get; }

    public StubHost(string host)
    {
        Host = new Uri(host, UriKind.Absolute);
        StubRequests = new List<StubRequest>();
    }

    public async Task<HttpResponseMessage> CreateResponseAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var stubRequest in StubRequests)
        {
            var httpResponseAsync =
                await stubRequest.CreateHttpResponseAsync(request, cancellationToken);

            if (httpResponseAsync != null)
            {
                return httpResponseAsync;
            }
        }

        return null!;
    }

    public bool IsPathRegistered(HttpRequestMessage request)
    {
        return StubRequests.Exists(x => x.DoesMatchFilterAsync(request).Result);
    }

    public IEnumerable<StubRequest> RegisteredRequests()
    {
        return StubRequests;
    }

    public IStubRequest Request(string path, HttpMethod method)
    {
        var req = new StubRequest(this, path, method);
        StubRequests.Add(req);
        return req;
    }

    public IStubRequest Get(string path) => Request(path, HttpMethod.Get);

    public IStubRequest Post(string path) => Request(path, HttpMethod.Post);

    public IStubRequest Put(string path) => Request(path, HttpMethod.Put);

    public IStubRequest Delete(string path) => Request(path, HttpMethod.Delete);
}
