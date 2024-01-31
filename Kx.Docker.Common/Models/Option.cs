namespace Kx.Docker.Common.Models;

public class Option
{
    public Option(string name, string image, Dictionary<string, string> env)
    {
        Name = name;
        Image = image;
        Env = env;
    }

    public string Name { get; set; }
    public string Image { get; set; }
    public Dictionary<string, string> Env { get; set; }
}

