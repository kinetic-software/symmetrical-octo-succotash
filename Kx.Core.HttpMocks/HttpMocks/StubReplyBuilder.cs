using System.Net;
using System.Text;

namespace Kx.Core.HttpMocks.HttpMocks;

internal class StubReplyBuilder
{
    private HttpStatusCode _status = HttpStatusCode.OK;
    private readonly List<(string, string)> _headers = new();
    private Func<HttpContent> _content = null!;
    private Exception _exception = null!;
    private TimeSpan _delay;
    private readonly List<Func<HttpRequestMessage, Task>> _executeTasks = new();

    private static readonly Encoding Encoding = new UTF8Encoding(false);

    public void WithHeader(string name, string value)
    {
        _headers.Add((name, value));
    }

    public void Delay(int milliseconds)
    {
        _delay = TimeSpan.FromMilliseconds(milliseconds);
    }

    public void Delay(TimeSpan time)
    {
        _delay = time;
    }

    public async Task<HttpResponseMessage> Build(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await Task.WhenAll(_executeTasks.Select((Func<Func<HttpRequestMessage, Task>, Task>) (x => x(request))));
        
        if (_delay != TimeSpan.Zero) await Task.Delay(_delay, cancellationToken);
        if (_exception != null) throw _exception;
        
        var httpResponseMessage1 = new HttpResponseMessage(_status);
        var content = _content;
        
        httpResponseMessage1.Content = content();
        var httpResponseMessage2 = httpResponseMessage1;

        foreach (var (name, str) in _headers)
        {
            httpResponseMessage2.Headers.TryAddWithoutValidation(name, str);
        }

        return httpResponseMessage2;
    }

    public void NoReply()
    {
        _delay = Timeout.InfiniteTimeSpan;
    }

    public void Throws(Exception exception)
    {
        _exception = exception;
    }

    public void Reply(HttpStatusCode code)
    {
        WithStatusCode(code);
    }

    public void Reply(HttpStatusCode code, object body)
    {
        WithStatusCode(code).WithBody(body);
    }

    public void Reply(HttpStatusCode code, string content, string contentType)
    {
        WithStatusCode(code).WithBody(content, contentType);
    }

    public void Execute(Action<HttpRequestMessage> action)
    {
        _executeTasks.Add(x =>
        {
            action(x);
            return Task.CompletedTask;
        });
    }

    public void Execute(Func<HttpRequestMessage, Task> action)
    {
        _executeTasks.Add(action);
    }

    private void WithBody(object body)
    {
        _content = () => new JsonContent(body);
    }

    private void WithBody(string content, string contentType)
    {
        _content = () => new StringContent(content, Encoding, contentType);
    }

    private StubReplyBuilder WithStatusCode(HttpStatusCode code)
    {
        _status = code;
        return this;
    }
}
