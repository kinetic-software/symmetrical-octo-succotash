namespace Kx.Core.HttpMocks.HttpMocks;

public interface IStubHost
{
    Uri Host { get; }

    IStubRequest Get(string path);

    IStubRequest Post(string path);

    IStubRequest Put(string path);

    IStubRequest Delete(string path);
}
