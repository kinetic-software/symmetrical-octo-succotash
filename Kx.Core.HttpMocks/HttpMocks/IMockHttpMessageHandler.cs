namespace Kx.Core.HttpMocks.HttpMocks;

public interface IMockHttpMessageHandler
{
    IReadOnlyCollection<IStubHost> Hosts { get; }
    IStubHost Host(string? hostUri);
}