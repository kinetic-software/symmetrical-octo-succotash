namespace Kx.Availability.Data.Interface.Interfaces;

public interface IEvent
{
    Task Process(IEventModel e, IDataAccessFactory dataAccessFactory);
}