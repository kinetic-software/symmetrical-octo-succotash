using System.Diagnostics.CodeAnalysis;
using Kx.Core.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
        if (!string.IsNullOrEmpty(context?.Request.Path))
        {                        
            var tenantId = context.GetRouteData().Values["tenantId"] as string;

            if (string.IsNullOrEmpty(tenantId))
            {
                throw new BadHttpRequestException("Path does not contain TenantId");
            }

            // DC - This is quite "loosely typed", assuming all TenantId's are integers (for now) we should / could be validating the data is a number; a simple Int.TryParse(... would suffice prior
            //      to setting it.
            TenantId = tenantId;                            
        }
        else
        {
            //DC: Would have also tested for "whitespace" but upstream in my test I found out
            //    when "mocking" setting the Path to an empty string e.g " " - you actually get
            //    an ArgumentException thrown so it's a test the runtime simply wont allow me to cover!
            throw new BadHttpRequestException("Request path cannot be null or empty");
        }
    }
}

