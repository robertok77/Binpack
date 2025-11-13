using Binpack.Shared.Models;
using Binpack.UI.State.Abstractions;
using Binpack.UI.State.HarborDomain;

namespace Binpack.Web.Abstractions;

public interface IPortVM
{
    IHarbor State { get; }
    bool HasData { get; }
    event Action? StateChanged;
    Task<PortDto> InitializeAsync(PortDto? portDto = null);
    Task UpdateStateAsync(DomRectDto anchorageRect);
    void StartDrag(Vessel v, DimDto client, DimDto offset);
    void OnDrag(DimDto client);
    void EndDrag(DimDto client);
    void Rotate(Vessel v);
    void Reset();
    Task New();
}