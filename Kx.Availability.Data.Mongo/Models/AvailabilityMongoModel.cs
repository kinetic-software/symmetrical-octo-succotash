using Kx.Core.Common.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace Kx.Availability.Data.Mongo.Models;

[BsonIgnoreExtraElements]
public class AvailabilityMongoModel : IEntity, ITenantDataModel
{
    public string TenantId { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public int? CommercialRoomTypeId { get; set; }
    public MetaModel Meta { get; init; } = new();
    public List<LocationModel> Locations { get; set; } = new();
    public int DisplayOrder;
    
    [BsonId] public string? ID { get; set; }

    public object GenerateNewID() => ObjectId.GenerateNewId().ToString();

    public bool HasDefaultID()
    {
        return false;
    }
}