namespace Kx.Availability.Data.Interface.Interfaces;

public interface ITenant
{    
    public string TenantId { get; }

    public string Jurisdiction { get; }
}