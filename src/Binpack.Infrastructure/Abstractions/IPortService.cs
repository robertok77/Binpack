using Binpack.Shared.Models;

namespace Binpack.Infrastructure.Abstractions;

public interface IPortService
{
    Task<PortDto?> GetPortDataAsync();
}