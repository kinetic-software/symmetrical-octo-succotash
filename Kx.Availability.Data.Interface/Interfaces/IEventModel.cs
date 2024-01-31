namespace Kx.Availability.Data.Interface.Interfaces;

public interface IEventModel
{
    string RawKrn { get; set; }
    IKrn Krn { get; }
    string SchemaVersion { get; set; }
    string ChangeType { get; set; }
    int ResourceVersion { get; set; }
    dynamic Data { get; set; }
}

public interface IEventDataModel {}

public interface IKrn
{
    string Partition { get; }
    string Service { get; }
    string Jurisdiction { get; }
    string Tenant { get; }
    string ResourceType { get; }
    string ResourceId { get; }
    
}
