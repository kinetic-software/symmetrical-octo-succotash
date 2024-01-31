using Kx.Docker.Common.Shared;

namespace Kx.Docker.Common.Models;

public class DockerContainer : IDockerContainer
{
    // ID is the id of the docker container
    public string Id { get; set; } = string.Empty;

    // Host is the IP of the container
    public string Host { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    // SAPassword is the password stored in the env of the container
    public string SaPassword { get; set; } = string.Empty;

    // Port is the port that the container is listening to
    public int Port { get; set; }
    public string Server { get; set; } = string.Empty;

    // To Check whether Container was created or not
    public bool IsCreated { get; set; }
}

