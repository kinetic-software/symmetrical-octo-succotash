using Kx.Core.Common.Exceptions;
using Kx.Core.Common.Interfaces;

namespace Kx.Core.Common.Data;

public static class DataAccessHelper
{
    public static IDataAccessAggregation ParseAggregationDataAccess(IDataAccess dataAccess)
    {
        return dataAccess as IDataAccessAggregation ?? throw new UnprocessableEntityException();
    } 
}