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
            //DC: I see this is commented out, but I also see by default the JsonSerializerSettings format would be "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"
            //    If I am not mistaken, deciding to uncomment this could give us mixed (and unstable) results with our test(s) which have a "StartTime" 
            //    of the same Year/Month and Day - because ordering would then be by chance IF we choose to ignore the hours/minutes/seconds etc
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