using Exiled.API.Interfaces;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
}
