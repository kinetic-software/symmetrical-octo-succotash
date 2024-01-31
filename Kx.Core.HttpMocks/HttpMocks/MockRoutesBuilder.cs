using System.Net;

namespace Kx.Core.HttpMocks.HttpMocks
{
    public class MockRouteBuilder : IMockRouteBuilder
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MockRouteBuilder(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IStubRequest? MockGetEndpoint(string route,
            string type,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType)
        {
            _httpClientFactory.CreateClient(type);

            var getRequest = (_httpClientFactory as MockHttpClientFactory)?
                .Host
                .Get(route);

            getRequest?.Reply(statusCode, returnValue, contentType);

            return getRequest;
        }

        public IStubRequest? MockPostEndpoint(string route,
            string type,
            string body,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType)
        {
            _httpClientFactory.CreateClient(type);

            var postRequest = (_httpClientFactory as MockHttpClientFactory)?
                .Host
                .Post(route)
                .WithBody(body);

            postRequest?.Reply(statusCode, returnValue, contentType);

            return postRequest;
        }

        public IStubRequest? MockPutEndpoint(string route,
            string type,
            string body,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType)
        {
            _httpClientFactory.CreateClient(type);

            var putRequest = (_httpClientFactory as MockHttpClientFactory)?
                .Host
                .Put(route)
                .WithBody(body);

            putRequest?.Reply(statusCode, returnValue, contentType);

            return putRequest;
        }

        public IStubRequest? MockDeleteEndpoint(string route,
            string type,
            HttpStatusCode statusCode,
            string returnValue,
            string contentType)
        {
            _httpClientFactory.CreateClient(type);

            var deleteRequest = (_httpClientFactory as MockHttpClientFactory)?
                .Host
                .Delete(route);

            deleteRequest?.Reply(statusCode, returnValue, contentType);

            return deleteRequest;
        }
    }
}