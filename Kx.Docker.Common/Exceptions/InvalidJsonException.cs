namespace Kx.Docker.Common.Exceptions;

public class InvalidJsonException : Exception
{
    public InvalidJsonException(string message)
        : base($"could not decode json: err: {message}")
    {
    }
}
