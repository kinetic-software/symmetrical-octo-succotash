namespace Kx.Docker.Common.Exceptions;

public class NameRequiredException : Exception
{
    public NameRequiredException()
       : base("name required")
    { }
}

