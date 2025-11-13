using System.Net.Http.Json;
using System.Text.Json;
using Binpack.Infrastructure.Abstractions;
using Binpack.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Binpack.Infrastructure.Services;

public sealed class PortService(HttpClient httpClient, ILogger<PortService> logger) : IPortService
{
    private const string FleetsRandomEndpoint = "api/fleets/random";
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<PortService> logger = logger;

    public async Task<PortDto?> GetPortDataAsync()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<PortDto>(FleetsRandomEndpoint,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Retrieving data error");
            return null;
        }
        
    }
}