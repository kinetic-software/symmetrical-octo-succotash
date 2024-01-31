namespace Kx.Docker.Common.Shared;

public class ConnectionDefinition : IConnectionDefinition
{
    public string DatabaseName { get; set; }
    public string TenantId { get; init; }
    public string Jurisdiction { get; init; }
    public string? ConnectionString { get; private set; }
    
    public ConnectionDefinition(string connectionString, string tenantId, string jurisdiction)
    {
        ConnectionString = connectionString;
        TenantId = tenantId;
        Jurisdiction = jurisdiction;
        DatabaseName = string.Empty;
    }
}