using System.Net;

namespace Kx.Core.HttpMocks.HttpMocks;

public interface IMultipleReplyBuilderOptions
{
    IMultipleReplyBuilderOptions WithHeader(string name, string value);

    IMultipleReplyBuilderOptions Delay(int milliseconds);

    IMultipleReplyBuilderOptions Delay(TimeSpan time);

    IMultipleReplyBuilderOptions Execute(Action<HttpRequestMessage> action);

    IMultipleReplyBuilderOptions Execute(Func<HttpRequestMessage, Task> action);

    IMultipleReplyBuilder NoReply();

    IMultipleReplyBuilder Throws(Exception exception);

    IMultipleReplyBuilder Reply(HttpStatusCode code);

    IMultipleReplyBuilder Reply(HttpStatusCode code, object body);

    IMultipleReplyBuilder Reply(HttpStatusCode code, string content, string contentType);
}