using Kx.Core.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Kx.Availability.Tests.Data;

public class TestTenant : ITenant
{
    public string TenantId { get; set; } = string.Empty;
    public string Jurisdiction { get; set; } = string.Empty;

    public TestTenant(IConfiguration config)
    {
        config["MongoID"] = "mongo-guid-1234";
    }
}
