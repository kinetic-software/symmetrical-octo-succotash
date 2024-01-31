using CliWrap.Exceptions;
using Kx.Docker.Common;
using Kx.Docker.Common.DockerCreation;
using Kx.Docker.Common.Exceptions;
using Kx.Docker.Common.Models;
using Kx.Docker.Common.Shared;
using Newtonsoft.Json;
using Serilog;
// ReSharper disable StringLiteralTypo

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace Kx.Docker.Mongo;

public class MongoTestHelper : ITestHelper
{
    private readonly IDockerCreation _docker;

    public MongoTestHelper(IDockerCreation docker)
    {
        _docker = docker;
    }

    public async Task<IConnectionDefinition> CreateDbPoolAsync(string dbName)
    {
        try
        {
            var container = await CreateDockerContainerAsync();

            //Thread is put to sleep inorder for Docker Container to be started before creating the database.
            if (container.IsCreated)
            {
                Thread.Sleep(5000);
            }

            _docker.CreateConnectionString(container, "master");

            var rnd = new Random();
            dbName = dbName + "_" + rnd.Next(1, 50000);
            var conn = _docker.CreateConnectionString(container, dbName);

            return new ConnectionDefinition(conn, dbName, conn);

        }
        catch (CommandExecutionException e)
        {
            Log.Logger.Error(e.Message);
            throw new InspectContainerException(e.Message);
        }
        catch (JsonReaderException e)
        {
            Log.Logger.Error(e.Message);
            throw new InvalidJsonException(e.Message);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
            throw;
        }
    }

    private async Task<IDockerContainer> CreateDockerContainerAsync()
    {
        var container = new DockerContainer();
        var options = _docker.GetDockerOptions();

        if (Environment.GetEnvironmentVariable("ENVIRONMENT") != null)
        {
            container.UserName = "admin";
            container.SaPassword = "MongoAdmin";
            container.Id = "test";
            container.IsCreated = true;
            container.Port = 27017;
            container.Server = "mongodbserver";
        }
        else
        {
            container = await _docker.StartContainerAsync(options) as DockerContainer;
        }

        if (string.IsNullOrEmpty(container?.Id))
            throw new ContainerNotFoundException(
                $"{options.Name} Encountered some error while getting Container"
            );

        return container;
    }
}
