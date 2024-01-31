using Kx.Availability.Data.Mongo.Data;
using Kx.Availability.Tests.Data;
using Kx.Docker.Common;
using Kx.Docker.Common.DockerCreation;
using Kx.Docker.Mongo;
using Kx.Docker.Mongo.DockerCreation;
using MongoDB.Driver;
using MongoDB.Entities;
using Kx.Core.Common.Data.MongoDB;
using Kx.Core.Common.Data;

namespace Kx.Availability.Tests;

public class TestConnectionDefinitionFactory : IConnectionDefinitionFactory
{
    private readonly IMongoSettings _mongoSettings;

    public TestConnectionDefinitionFactory(IMongoSettings mongoSettings)
    {
        _mongoSettings = mongoSettings;
    }


    public async Task<(string ConnectionString, string DatabaseName)> CreateTestMongoDatabase(string databaseName)
    {
        if (databaseName == null)
        {
            throw new ArgumentNullException(nameof(databaseName));
        }

        try
        {
            /*For now we create the test helper etc. here as we have multiple different implementations
             * for them depending on the type of connection (Mongo vs Sql vs Dynamo).
             * */
            IDockerCreation docker = new MongoDockerCreation();            
            ITestHelper testHelper = new MongoTestHelper(docker);

            /* Because each feature runs in it's own thread it's possible
             * that each thread can try and create the Docker Container and database at the same time and cause conflicts.
             * So we use a Mutex here to ensure that only one thread can do this at the same time. This means
             * the second thread would then reuse the same docker container and not create a new one.
            */
            ThreadManager.ConnectionMutex.WaitOne();

            var connectionString = ((TestMongoSettings)_mongoSettings).ConnectionString;

            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains(databaseName))
                return (connectionString, "test");

            var createdDb = (await testHelper.CreateDbPoolAsync(databaseName));

            connectionString = createdDb.ConnectionString!;
            ((TestMongoSettings)_mongoSettings).ConnectionString = createdDb.ConnectionString!;

            await InitStaticMongoDbAsync(databaseName, connectionString);

            return (connectionString, createdDb.DatabaseName);
        }
        finally
        {
            /* Release the Mutex so that the next test thread can now get it's connection string. */
            ThreadManager.ConnectionMutex.ReleaseMutex();
            
        }
    }

    private async Task InitStaticMongoDbAsync(string databaseName, string connectionString)
    {
        await DB.InitAsync(
            databaseName,
            MongoClientSettings.FromConnectionString(connectionString)
        );
    }

    public IMongoDbConnection GetMongoDbConnection()
    {
        var connection = new MongoDbConnection(_mongoSettings);        
        return connection;

    }
}
