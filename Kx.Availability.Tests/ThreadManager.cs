using TechTalk.SpecFlow;

namespace Kx.Availability.Tests;

/// <summary>
/// This class manages the Mutex  used during the tests to prevent
/// the threads from interacting with each other.
/// </summary>
internal static class ThreadManager 
{

    private static Mutex _connectionMutex = new Mutex();

    /// <summary>
    /// A mutex to manage the database connection string, prevents
    /// multiple threads from creating a docker container while another
    /// thread is also doing it.
    /// </summary>
    internal static Mutex ConnectionMutex => _connectionMutex;

    /// <summary>
    /// Once all the tests are complete make sure we dispose the mutex.
    /// </summary>
    [AfterTestRun]
    internal static void Cleanup() {
        _connectionMutex.Dispose();
    }


}