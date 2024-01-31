using System.Text.Json;
using Newtonsoft.Json;

namespace Kx.Core.Common.HelperClasses;

public interface IKxJsonSettings
{
    JsonSerializerSettings SerializerSettings { get; init; }
    JsonSerializerOptions SerializerOptions { get; init; }
}