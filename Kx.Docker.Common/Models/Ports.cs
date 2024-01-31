using Newtonsoft.Json;

namespace Kx.Docker.Common.Models;

public class Ports
{
    [JsonProperty(PropertyName = "1433/tcp")]
    public Tcp[]? Tcp1433 { get; set; }
    
    [JsonProperty(PropertyName = "27017/tcp")]
    public Tcp[]? Tcp27017 { get; set; }
    
    [JsonProperty(PropertyName = "8000/tcp")]
    public Tcp[]? Tcp8000 { get; set; }
}