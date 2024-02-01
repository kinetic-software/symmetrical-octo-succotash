using Kx.Core.Common.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace Kx.Availability.Data.Mongo.Models;

// ReSharper disable once ClassNeverInstantiated.Global
[BsonIgnoreExtraElements]
public class DataLoadStateModel : IEntity, ITenantDataModel
{
    public string TenantId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime StateTime { get; set; } = DateTime.UtcNow;
    public string? State { get; set; }
    public string? ExceptionMessage { get; set; }
    
    public bool IsEnded { get; set; } = false;
    
    [BsonId] public string ID { get; set; } = ObjectId.GenerateNewId().ToString();
    public object GenerateNewID() => ObjectId.GenerateNewId().ToString();

    public bool HasDefaultID()
    {
        return false;
    }
}
