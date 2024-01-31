using System.Text;

namespace Kx.Availability.Data.Interface.Exceptions;

public static class ExceptionExtensions
{
    public static string FullMessage(this Exception exception)
    {
        var messages = new StringBuilder();

        var innerException = exception;
        do
        {
            messages.AppendLine(innerException.Message);
            innerException = innerException.InnerException;
        } while (innerException != null);

        return messages.ToString();
    }
}