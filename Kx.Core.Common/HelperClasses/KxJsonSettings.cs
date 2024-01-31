using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kx.Core.Common.HelperClasses;


public class KxJsonSettings : IKxJsonSettings
{
    public JsonSerializerSettings SerializerSettings { get; init; }
    public JsonSerializerOptions SerializerOptions { get; init; }

    public KxJsonSettings()
    {
        SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };
        SerializerSettings.Converters.Add(new StringEnumConverter());
        
        SerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}