using System.Net;
using Kx.Core.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kx.Core.Common.HelperClasses;

public static class ReturnResults
{
    public static IResult Result((HttpStatusCode StatusCode, string Results) results)
    {
        return results.StatusCode switch
        {
            HttpStatusCode.OK => Results.Ok(results.Results),
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.NotFound => Results.NotFound(results.Results),
            HttpStatusCode.ExpectationFailed => Results.Problem(results.Results),
            HttpStatusCode.UnprocessableEntity => Results.UnprocessableEntity(results.Results),
            _ => Results.Problem()
        };
    }
}
