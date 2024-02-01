using System.Diagnostics.CodeAnalysis;
using Kx.Core.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace Kx.Availability.Data.Connection;

public class Tenant : ITenant
{
    public string TenantId { get; private set; }    

    public Tenant(IHttpContextAccessor httpContextAccessor)
    {
        LoadTenant(httpContextAccessor);
    }

    [MemberNotNull(nameof(TenantId))]    
    private void LoadTenant(IHttpContextAccessor httpContextAccessor)
    {        
        var context = httpContextAccessor.HttpContext;
        if (context?.Request.Path != null)
        {                        
            var tenantId = context.GetRouteData().Values["tenantId"] as string;

            if (string.IsNullOrEmpty(tenantId))
            {
                throw new BadHttpRequestException("Path does not contain TenantId");
            }

            TenantId = tenantId;                            
        }
        else
        {
            throw new BadHttpRequestException("Path does not contain TenantId");
        }
    }
}

