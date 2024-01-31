using System.Net;

namespace Kx.Core.HttpMocks.HttpMocks
{
 public interface IMockRouteBuilder
    {
        IStubRequest? MockGetEndpoint(string route,
            string type,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType);

        IStubRequest? MockPostEndpoint(string route,
            string type,
            string body,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType);

        IStubRequest? MockPutEndpoint(string route,
            string type,
            string body,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType);

        IStubRequest? MockDeleteEndpoint(string route,
            string type,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType);

    }
}
