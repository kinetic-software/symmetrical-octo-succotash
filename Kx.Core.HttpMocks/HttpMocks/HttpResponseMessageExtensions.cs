namespace Kx.Core.HttpMocks.HttpMocks;

internal static class HttpResponseMessageExtensions
{
    public static async Task<HttpResponseMessage> CloneAsync(this HttpResponseMessage response)
    {
        var clone = response != null ? new HttpResponseMessage(response.StatusCode) : throw new ArgumentNullException(nameof (response));
        {
            var ms = new MemoryStream();
            
            await response.Content.LoadIntoBufferAsync();
            await response.Content.CopyToAsync(ms);
            
            ms.Position = 0L;
            clone.Content = new StreamContent(ms);
            
            foreach (var header in response.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            await ms.DisposeAsync();
        }
        
        clone.ReasonPhrase = response.ReasonPhrase;
        clone.Version = response.Version;
        clone.RequestMessage = response.RequestMessage;
        
        foreach (var header in response.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}