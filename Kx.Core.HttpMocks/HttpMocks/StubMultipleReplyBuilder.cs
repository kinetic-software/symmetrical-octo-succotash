using System.Collections.Concurrent;
using System.Net;

namespace Kx.Core.HttpMocks.HttpMocks;

internal class StubMultipleReplyBuilder : IMultipleReplyBuilder, IMultipleReplyBuilderOptions
  {
    private readonly ConcurrentQueue<StubReplyBuilder> _responses;
    private StubReplyBuilder _current;

    public StubMultipleReplyBuilder(ConcurrentQueue<StubReplyBuilder> responses)
    {
      _responses = responses;
      _current = new StubReplyBuilder();
    }

    private void CreateNewReplyBuilder()
    {
      _responses.Enqueue(_current);
      _current = new StubReplyBuilder();
    }

    public IMultipleReplyBuilder Throws(Exception exception)
    {
      _current.Throws(exception);
      CreateNewReplyBuilder();
      return this;
    }

    public IMultipleReplyBuilderOptions WithHeader(string name, string value)
    {
      _current.WithHeader(name, value);
      return this;
    }

    public IMultipleReplyBuilderOptions Delay(int milliseconds)
    {
      _current.Delay(milliseconds);
      return this;
    }

    public IMultipleReplyBuilderOptions Delay(TimeSpan time)
    {
      _current.Delay(time);
      return this;
    }

    public IMultipleReplyBuilder NoReply()
    {
      _current.NoReply();
      CreateNewReplyBuilder();
      return this;
    }

    public IMultipleReplyBuilder Reply(HttpStatusCode code)
    {
      _current.Reply(code);
      CreateNewReplyBuilder();
      return this;
    }

    public IMultipleReplyBuilder Reply(HttpStatusCode code, object body)
    {
      _current.Reply(code, body);
      CreateNewReplyBuilder();
      return this;
    }

    public IMultipleReplyBuilder Reply(HttpStatusCode code, string content, string contentType)
    {
      _current.Reply(code, content, contentType);
      CreateNewReplyBuilder();
      return this;
    }

    public IMultipleReplyBuilderOptions Execute(Action<HttpRequestMessage> action)
    {
      _current.Execute(action);
      return this;
    }

    public IMultipleReplyBuilderOptions Execute(Func<HttpRequestMessage, Task> action)
    {
      _current.Execute(action);
      return this;
    }
  }