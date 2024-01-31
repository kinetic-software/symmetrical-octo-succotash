using Kx.Availability.Data.Interface.Interfaces;

namespace Kx.Availability.Data.Interface.HelperClasses;

public static class DataAccessHelper
{
    public static IDataAggregationAccess ParseDataAggregationAccess(IDataAccess dataAccess)
    {
        return dataAccess as IDataAggregationAccess ?? throw new Exception();
    }
}