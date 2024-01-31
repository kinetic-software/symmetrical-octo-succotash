using Microsoft.AspNetCore.Http;

namespace Kx.Availability.Data.Interface.Interfaces;

public interface IConnectionReader
{
    Task InvokeAsync(
          HttpContext httpContext,
          IConnectionDefinitionFactory connectionDefinitionFactory
      );
}