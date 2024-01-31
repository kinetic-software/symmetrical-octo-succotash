namespace Kx.Docker.Common.Shared;

public interface IConnectionDefinition
{
    string DatabaseName { get; set; }
    string TenantId { get; init; }    
    string? ConnectionString { get; }
}