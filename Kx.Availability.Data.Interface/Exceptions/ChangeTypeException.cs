namespace Kx.Availability.Data.Interface.Exceptions;

public class ChangeTypeParseException : Exception
{
    public ChangeTypeParseException()
        : base("Failed to parse Change Type")
    {
    }

    public ChangeTypeParseException(string changeType)
        : base($"Failed to parse Change Type: {changeType}")
    {
    }
}