using System.Text.Json.Serialization;

namespace Kx.Availability.Data.Mongo.Models;

public class LocationModel
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsDirectLocation { get; set; }  = false;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentId { get; set; } = null;
    public MetaModel Meta { get; set; } = new();
    
}
