namespace Kx.Docker.Common.Shared;

public interface IDockerContainer
{
    string Id { get; set; }
    string Host { get; set; }
    string UserName { get; set; }
    string SaPassword { get; set; }
    int Port { get; set; }
    string Server { get; set; }
    bool IsCreated { get; set; }
}