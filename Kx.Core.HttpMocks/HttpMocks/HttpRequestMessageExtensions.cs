#pragma warning disable CS0618
namespace Kx.Core.HttpMocks.HttpMocks;

internal static class HttpRequestMessageExtensions
{
    public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof (request));
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);
        
        if (request.Content != null)
        {
            var ms = new MemoryStream();
            await request.Content.CopyToAsync(ms);
            ms.Position = 0L;
            clone.Content = new StreamContent(ms);
            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            await ms.DisposeAsync();
        }
        clone.Version = request.Version;
        
        foreach (KeyValuePair<string, object?> property in request.Options.AsEnumerable())
            if (clone.Options != null)
                clone.Options.Set(new HttpRequestOptionsKey<object?>(property.Key), property.Value);

        var cloneOptions = Array.Empty<KeyValuePair<string, object?>>();
        ((ICollection<KeyValuePair<string, object?>>)request.Options).CopyTo(cloneOptions, 0);

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}