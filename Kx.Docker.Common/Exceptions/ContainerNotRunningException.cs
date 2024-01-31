namespace Kx.Docker.Common.Exceptions;

public class ContainerNotRunningException : Exception
{
    public ContainerNotRunningException()
        : base("Container is created but not running")
    { }
}

