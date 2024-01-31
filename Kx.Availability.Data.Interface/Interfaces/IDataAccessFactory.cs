using Kx.Availability.Data.Interface.Enums;

namespace Kx.Availability.Data.Interface.Interfaces;

public interface IDataAccessFactory
{
    IDataAccess GetDataAccess(KxDataType kxDataType);
    IDataAggregationStoreAccess<T> GetDataStoreAccess<T>() where T : class;
}