using System.Diagnostics.CodeAnalysis;
using Kx.Core.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace Kx.Availability.Data.Connection;

public class Tenant : ITenant
{
    public string TenantId { get; private set; }    

    private const int _secondsToCacheTenant = 5;
    private static readonly object _tenantLock = new object();
    private static List<Tenant> Tenants { get; set; } = new List<Tenant>();
    private DateTime TenantTimeStamp { get; set; }


    public Tenant(IHttpContextAccessor httpContextAccessor)
    {
        TenantTimeStamp = DateTime.Now;
        LoadTenant(httpContextAccessor);
    }

    [MemberNotNull(nameof(TenantId))]    
    private void LoadTenant(IHttpContextAccessor httpContextAccessor)
    {
        Log.Information("checking tenant");
        var context = httpContextAccessor.HttpContext;
        if (context?.Request.Path != null)
        {

            lock (_tenantLock)
            {
                /* Get the tenant Id from the url */
                var tenantId = context.GetRouteData().Values["tenantId"] as string;

                if (string.IsNullOrEmpty(tenantId))
                {
                    throw new BadHttpRequestException("Path does not contain TenantId");
                }

                if (Tenants.All(x => x.TenantId != tenantId) || Tenants.Any(x => x.TenantId == tenantId && HasTenantCacheExpired(x)))
                {
                    TenantId = tenantId;                    
                                        
                    Tenants.RemoveAll(x => x.TenantId == tenantId);
                    Tenants.Add(this);                    
                }
                else
                {
                    var tenant = Tenants.First(x => x.TenantId == tenantId);
                    TenantId = tenant.TenantId;                    
                }

            }
        }
        else
        {
            throw new BadHttpRequestException("Path does not contain TenantId");
        }
    }

    private bool HasTenantCacheExpired(Tenant tenant)
    {
        return (DateTime.UtcNow - tenant.TenantTimeStamp).TotalSeconds > _secondsToCacheTenant;
    }
}

