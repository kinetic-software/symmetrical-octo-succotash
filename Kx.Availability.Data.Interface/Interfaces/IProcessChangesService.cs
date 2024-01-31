namespace Kx.Availability.Data.Interface.Interfaces;

public interface IProcessChangesService
{
    Task ProcessChange(IEventModel e);
}