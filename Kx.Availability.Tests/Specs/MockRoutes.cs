namespace Kx.Availability.Tests.Specs
{
    internal static class MockRoutes
    {
        internal static string GetLocationsRoute(string tenantId)
        {
            return $"production/v1/{tenantId}/locations";
        }

        internal static string GetLocationIdsRoute(string tenantId)
        {
            return $"production/v1/{tenantId}/locations/ids";
        }

    }
}
