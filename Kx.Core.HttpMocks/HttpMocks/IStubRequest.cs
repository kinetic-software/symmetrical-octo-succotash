using System.Net;

namespace Kx.Core.HttpMocks.HttpMocks;

public interface IStubRequest
{
    bool Called { get; }

    string Path { get; }

    IStubRequest WithBody(object body);

    void Reply(HttpStatusCode code, string content, string contentType);
}