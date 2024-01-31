namespace Kx.Docker.Common.Exceptions;

public class ContainerNotFoundException : Exception
{

    public ContainerNotFoundException(string name)
        : base($"Container having this name: {name}  Not Found")
    {
    }
}
