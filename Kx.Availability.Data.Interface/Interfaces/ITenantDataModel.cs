namespace Kx.Availability.Data.Interface.Interfaces;

public interface ITenantDataModel : IDataModel
{
    string TenantId { get; set; }
}