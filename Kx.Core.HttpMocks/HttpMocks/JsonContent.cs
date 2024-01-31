using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Kx.Core.HttpMocks.HttpMocks;

public class JsonContent : StreamContent
{
    private static readonly Encoding Encoding = new UTF8Encoding(false);

    public JsonContent(object obj)
        : this(obj, JsonSerializer.CreateDefault())
    {
    }

    private JsonContent(object obj, JsonSerializer serializer)
        : base(GetJsonStream(obj, serializer))
    {
        Headers.ContentType = new MediaTypeHeaderValue("application/json")
        {
            CharSet = Encoding.UTF8.WebName
        };
    }

    private static Stream GetJsonStream(object obj, JsonSerializer serializer)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof (obj));
        var jsonStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(jsonStream, Encoding, 1024, true))
        {
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                serializer.Serialize(jsonTextWriter, obj);
        }
        jsonStream.Position = 0L;
        return jsonStream;
    }
}