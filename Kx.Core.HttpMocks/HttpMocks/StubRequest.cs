using System.Collections.Concurrent;
using System.Net;
using Newtonsoft.Json.Linq;
using SpecFlow.Internal.Json;

namespace Kx.Core.HttpMocks.HttpMocks;

public class StubRequest : IStubRequest
{
  private readonly HttpMethod _method;
  private readonly Uri _uri;
  private StubReplyBuilder? _builder;
  private ConcurrentQueue<StubReplyBuilder>? _builders;
  private Func<HttpRequestMessage, Task<bool>>? _body;
  private bool _called;

  public StubRequest(StubHost stubHost, string path, HttpMethod method)
  {
    _uri = new Uri(stubHost.Host, new Uri(path, UriKind.Absolute));
    _method = method;
  }

  public IStubRequest WithBody(object body)
  {
    var content = new JsonContent(body);
    _body =
      async r =>
      {
          var flag = r.Content?.Headers.ContentType?.MediaType == "application/json";

          if (!flag) return flag;

          var str = await content.ReadAsStringAsync();
          if (r.Content != null)
          {
              var expected = JObject.Parse(await r.Content.ReadAsStringAsync());
              var actual = JObject.Parse(str.FromJson<string>());

              RemoveNullValues(expected);
              RemoveNullValues(actual);

              return JToken.DeepEquals(expected, actual);
          }

          return true;
      };
    return this;
  }

  public IStubRequest WithBody(string content, string contentType)
  {
    _body = (Func<HttpRequestMessage, Task<bool>>)
      (async r =>
      {
        var flag = r.Content?.Headers.ContentType?.MediaType == contentType;
        if (!flag) return flag;

        var str = content;
        if (r.Content != null) flag = str == await r.Content.ReadAsStringAsync();

        return flag;
      });
    return this;
  }

  internal async Task<bool> DoesMatchFilterAsync(HttpRequestMessage request)
  {
    if (request.RequestUri != _uri
        || request.Method != _method)
    {
      return false;
    }

    if (_body == null) return true;
    return await _body(request);
  }

  private async Task<bool> ShouldRespondAsync(HttpRequestMessage request) =>
    await DoesMatchFilterAsync(request) && (_builder == null && _builders?.Count > 0 || _builder != null);

  public async Task<HttpResponseMessage> CreateHttpResponseAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (!await ShouldRespondAsync(request))
      return null!;

    var clonedRequest = await request.CloneAsync();
    cancellationToken.ThrowIfCancellationRequested();

    HttpResponseMessage response;
    if (_builders != null && _builders.TryDequeue(out var result))
    {
      response = await result.Build(clonedRequest, cancellationToken);
    }
    else
    {
      response = await _builder!.Build(clonedRequest, cancellationToken);
    }

    var httpResponseMessage = await response.CloneAsync();
    httpResponseMessage.RequestMessage = clonedRequest;
    return response;
  }

  public bool Called => _called;

  public string Path => _uri.AbsolutePath;

  internal HttpMethod Method => _method;

  public void Reply(HttpStatusCode code, string content, string contentType)
  {
    _builders = null;
    _builder = new StubReplyBuilder();
    _builder.Reply(code, content, contentType);
    _called = true;
  }

  static void RemoveNullValues(JToken token)
  {
    var propertiesToRemove = token.Children<JProperty>()
        .Where(p => p.Value.Type == JTokenType.Null)
        .ToList();

    foreach (var property in propertiesToRemove)
    {
      property.Remove();
    }
  }
}