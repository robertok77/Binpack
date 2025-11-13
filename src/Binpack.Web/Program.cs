namespace Binpack.Web;

public static partial class Program
{
    public static async Task Main(string[] args) =>
        await WebApplication.CreateBuilder(args)
            .ConfigureServices()
            .Build()
            .Configure()
            .RunAsync();
}