using System.Text.Json.Serialization;

namespace Kx.Availability.Data.Mongo.Models;

public class MetaModel
{
    public int GermId { get; set; }
    [JsonIgnore]
    public int EntityVersion { get; set; }
}
