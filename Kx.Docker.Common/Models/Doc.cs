

// ReSharper disable ClassNeverInstantiated.Global

namespace Kx.Docker.Common.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class Doc
{
    public string Id { get; set; } = string.Empty;
    public State? State { get; set; }
    public Config? Config { get; set; }
    public NetworkSettings? NetworkSettings { get; set; }
}