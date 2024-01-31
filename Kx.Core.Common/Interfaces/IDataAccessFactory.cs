using Kx.Core.Common.Data;

namespace Kx.Core.Common.Interfaces;

public interface IDataAccessFactory
{    
    IDataAccess GetDataAccess(KxDataType kxDataType);
    IDataAggregationStoreAccess<T> GetDataStoreAccess<T>() where T : class;
}