using Kx.Docker.Common.Models;
using Kx.Docker.Common.Shared;

namespace Kx.Docker.Common.DockerCreation;

public interface IDockerCreation
{
    Task<IDockerContainer> StartContainerAsync(Option opt);
    Option GetDockerOptions();
    string CreateConnectionString(IDockerContainer dockerContainer, string name);
}

