namespace Kx.Docker.Common.Exceptions;

public class InspectContainerException : Exception
{
    public InspectContainerException(string message)
       : base($"failed to inspect container error: {message}")
    { }
}

