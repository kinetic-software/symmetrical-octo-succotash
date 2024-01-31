using Newtonsoft.Json;

namespace Kx.Availability.Data.Interface.Interfaces;

public interface IKxJsonSettings
{
    JsonSerializerSettings SerializerSettings { get; init; }
}