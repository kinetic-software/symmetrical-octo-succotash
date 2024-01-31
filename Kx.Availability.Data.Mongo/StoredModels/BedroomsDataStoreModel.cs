using Kx.Core.Common.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace Kx.Availability.Data.Mongo.StoredModels;

[BsonIgnoreExtraElements]

public class BedroomsDataStoreModel : IEntity, IDataStoreModel
{
    
    [BsonId] public string? ID { get; set; } = ObjectId.GenerateNewId().ToString();
   
    public object GenerateNewID() => ObjectId.GenerateNewId().ToString();

    public bool HasDefaultID()
    {
        return false;
    }

    public int GermId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BlockId { get; set; } //LocationID
    public int Capacity { get; set; } = 1;
    public int MaxCapacity { get; set; } = 1;
    public int GermTypeId { get; set; }
    public bool Inactive { get; set; }
}
