using Kx.Core.Common.Interfaces;

namespace Kx.Availability.Data.Mongo.StoredModels;


public class LocationsDataStoreModel : IDataStoreModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ExternalId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ParentId { get; set; } = null;
    public List<LocationsDataStoreModel> Locations { get; set; } = new ();
    public bool IsDirectLocation { get; set; } = false;
}
