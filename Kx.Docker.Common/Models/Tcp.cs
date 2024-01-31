using System.Diagnostics.CodeAnalysis;

namespace Kx.Docker.Common.Models;


[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class Tcp
{
    public string HostIp { get; set; } = string.Empty;
    public string HostPort { get; set; } = string.Empty;
}