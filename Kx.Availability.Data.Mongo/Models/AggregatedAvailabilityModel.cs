using Kx.Core.Common.Interfaces;

namespace Kx.Availability.Data.Mongo.Models;

public class AggregatedAvailabilityModel : ITenantDataModel
{
    public string TenantId { get; set; } = string.Empty;

    public readonly List<AvailabilityMongoModel> Availability = new();
    
    
}