namespace Kx.Availability.Data.Interface.Interfaces;

public interface IProcessChangesFactory
{
    Task ProcessChangeEvent(IEventModel e);
}