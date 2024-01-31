using System.Text.Json;
using System.Text.Json.Serialization;
using Kx.Core.Common.HelperClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kx.Availability.Tests.Dependencies;


public class KxJsonTestSettings : IKxJsonSettings
{
    public JsonSerializerSettings SerializerSettings { get; init; }
    public JsonSerializerOptions SerializerOptions { get; init; }

    public KxJsonTestSettings()
    {
        SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateParseHandling = DateParseHandling.DateTimeOffset,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            //DateFormatString = "yyyy-MM-dd"
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