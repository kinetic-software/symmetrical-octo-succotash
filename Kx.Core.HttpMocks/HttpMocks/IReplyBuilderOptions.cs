using System.Net;

namespace Kx.Core.HttpMocks.HttpMocks;

public interface IReplyBuilderOptions
{
    IReplyBuilderOptions WithHeader(string name, string value);

    IReplyBuilderOptions Delay(int milliseconds);

    IReplyBuilderOptions Delay(TimeSpan time);

    IReplyBuilderOptions Execute(Action<HttpRequestMessage> action);

    IReplyBuilderOptions Execute(Func<HttpRequestMessage, Task> action);

    IReplyBuilderOptions WithBody(object body);

    IReplyBuilderOptions WithBody(string content, string contentType);

    IReplyBuilderOptions WithStatusCode(HttpStatusCode code);

    IReplyBuilderOptions WithResponse(
        Func<HttpRequestMessage, HttpResponseMessage, CancellationToken, Task<HttpResponseMessage>>? responseDelegate);

    IReplyBuilderOptions WithResponse(
        Func<HttpRequestMessage, Task<HttpResponseMessage>> request);
}