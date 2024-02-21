using Serilog;
using Xunit.Abstractions;

namespace Kx.Availability.Tests.logging;

public abstract class LoggedTestSteps
{
    protected LoggedTestSteps(ITestOutputHelper testOutputHelper)
    {
        /* Get the test output helper for the scenario so we can setup logging against it. */
        Log.Logger = new LoggerConfiguration()
                .WriteTo.TestOutput(testOutputHelper)
                .CreateLogger();
    }
}