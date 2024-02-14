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
        //DC: Just trying to understand the whole CQRS format to this solution BUT feels "wrong" we are in what
        //    is deemed a "data" project and were attempting to access metadata / information from a HTTP request - should this logic not sit "upstream" - in Kx.Availability?
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

