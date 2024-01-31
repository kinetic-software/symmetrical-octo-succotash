namespace Kx.Core.Common.Interfaces
{
  public interface ITenantDataModel : IDataModel
  {
    string TenantId { get; set; }
  }
}