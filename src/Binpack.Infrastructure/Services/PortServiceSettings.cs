namespace Binpack.Infrastructure.Services;

public class PortServiceSettings
{
    public const string Section = "Instech.Service";
    public string BaseAddress { get; init; } = "";
    public int TimeoutSeconds { get; init; } = 30;
}