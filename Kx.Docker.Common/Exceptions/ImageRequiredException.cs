namespace Kx.Docker.Common.Exceptions;

public class ImageRequiredException : Exception
{
    public ImageRequiredException()
        : base("Image Required")
    { }
}

