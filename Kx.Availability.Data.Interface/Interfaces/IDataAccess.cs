using Kx.Availability.Data.Interface.Enums;

namespace Kx.Availability.Data.Interface.Interfaces;

public interface IDataAccess
{
    Task<IDataModel> Search(IEnumerable<SearchCriteria> filter);
    Task<IDataModel?> Get(IDataModel? data);
    Task Delete(IDataModel? data);
    Task Insert(IDataModel? data);
    Task Update(IDataModel? data);
}
