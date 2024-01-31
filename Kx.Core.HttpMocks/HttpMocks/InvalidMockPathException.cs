namespace Kx.Core.HttpMocks.HttpMocks;

public class InvalidMockPathException : Exception
{
    private readonly IEnumerable<StubRequest> _availablePaths;
    private readonly HttpRequestMessage _httpRequestMessage;

    public InvalidMockPathException(IEnumerable<StubRequest> availableRequests,
        HttpRequestMessage httpRequestMessage)
    {
        _availablePaths = availableRequests;
        _httpRequestMessage = httpRequestMessage;
    }

    public override string ToString()
    {
        string availablePathsString = string.Join(Environment.NewLine, _availablePaths.Select(x => $"{x.Path} ({x.Method})"));

        string message = $"Requested path {_httpRequestMessage.RequestUri?.AbsolutePath} ({_httpRequestMessage.Method}) has not been mocked. " +
            $"The available paths are: {Environment.NewLine}{availablePathsString}";

        return message;
    }

    public override string Message => ToString();

}
