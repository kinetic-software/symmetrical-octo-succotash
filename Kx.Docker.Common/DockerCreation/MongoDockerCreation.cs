using CliWrap;
using CliWrap.Buffered;
using CliWrap.Exceptions;
using Kx.Docker.Common.DockerCreation;
using Kx.Docker.Common.Exceptions;
using Kx.Docker.Common.Models;
using Kx.Docker.Common.Shared;
using Newtonsoft.Json;

namespace Kx.Docker.Mongo.DockerCreation;

public class MongoDockerCreation : IDockerCreation
{
    public async Task<IDockerContainer> StartContainerAsync(Option opt)
    {
        if (opt.Image == "")
        {
            throw new ImageRequiredException();
        }

        if (opt.Name == "")
        {
            throw new NameRequiredException();
        }

        var container = await GetContainerAsync(opt);

        /* No valid container found so we create a new one.*/
        if (string.IsNullOrEmpty(container.Id))
        {
            container = await CreateContainer(opt);
            container.IsCreated = true;
        }

        if (container == null)
        {
            throw new ContainerNotRunningException();
        }

        return container;
    }

    private async Task<IDockerContainer> CreateContainer(Option opt)
    {
        var cmdArgs = new List<string> { "run", "--rm", "-P", "-d", "--name", opt.Name };
        cmdArgs.AddRange(opt.Env.Select(env => $"-e {env.Key}={env.Value}"));

        cmdArgs.Add(opt.Image);

        try
        {
            await Cli.Wrap("docker")
                .WithArguments(string.Join(" ", cmdArgs))
                .ExecuteBufferedAsync();
        }
        catch (CommandExecutionException e)
        {
            // If both threads try to execute the start container command same time this will throw an error.
            // and in order to solve this problem we are calling getContainer for the second thread, because it
            // will be created by the first one.
            if (e.ExitCode != 125) throw;
            var container = await GetContainerAsync(opt);

            if (!string.IsNullOrEmpty(container.Id))
            {
                return container;
            }
            throw;
        }
        return await GetContainerAsync(opt);
    }

    private async Task<IDockerContainer> GetContainerAsync(Option opt)
    {
        var result = await Cli.Wrap("docker")
            .WithArguments("inspect " + opt.Name)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        switch (result.ExitCode)
        {
            case 1 when string.IsNullOrEmpty(result.StandardError):
                throw new ContainerNotFoundException(opt.Name);
            case 1 when !string.IsNullOrEmpty(result.StandardError):
                return new DockerContainer();
            default:
            {
                var container = GetContainerFromOutput(result.StandardOutput);
                return container;
            }
        }
    }

    private DockerContainer GetContainerFromOutput(string standardOutput)
    {
        var doc = JsonConvert.DeserializeObject<Doc[]>(standardOutput);
        var container = new DockerContainer();

        if (!ValidateDockerJson(doc)) return container;
        
        container.Server = "127.0.0.1";
        container.Id= doc![0].Id;
        container.Host = doc[0].NetworkSettings!.Ports!.Tcp27017![0].HostIp;

        var success = int.TryParse(
            doc[0].NetworkSettings!.Ports!.Tcp27017![0].HostPort,
            out var port
        );
        
        if (success)
        {
            container.Port = port;
        }
        else
        {
            throw new InvalidCastException("cannot convert network port to int");
        }

        foreach (var env in doc[0].Config!.Env!)
        {
            string[] parts = env.Split('=');
            if (parts[0].Equals("SA_PASSWORD"))
            {
                container.SaPassword = parts[1];
                break;
            }
        }

        return container;
    }

    public Option GetDockerOptions()
    {
        return new Option("mongo-testing", "mongo", new Dictionary<string, string>());
    }

    public string CreateConnectionString(IDockerContainer container, string name)
    {
        return $"mongodb://{container.Server}:{container.Port}/{name}";
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private bool ValidateDockerJson(Doc[]? doc)
    {
        if (doc == null || doc.Length == 0)
        {
            throw new JsonReaderException();
        }

        if (doc[0].State?.Running == false)
        {
            throw new ContainerNotRunningException();
        }

        if (doc[0].Config?.Env == null)
        {
            throw new InvalidJsonException("Docker Environment Variables are null.");
        }

        if (doc[0].Id == null)
        {
            throw new InvalidJsonException("Docker Id is null.");
        }

        if (doc[0].NetworkSettings?.Ports?.Tcp27017?[0].HostIp == null)
        {
            throw new InvalidJsonException("Host Ip is null.");
        }

        if (doc[0].NetworkSettings?.Ports?.Tcp27017?[0].HostPort == null)
        {
            throw new InvalidJsonException("Host Port is null.");
        }

        return true;
    }
}
