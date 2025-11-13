using Binpack.Infrastructure.Abstractions;
using Binpack.Shared.Models;
using Binpack.UI.State.Abstractions;
using Binpack.UI.State.HarborDomain;
using Binpack.Web.Abstractions;

namespace Binpack.Web.ViewModel;
/// <summary>
/// Represents the view model for managing port operations and state within a harbor system.
/// </summary>
/// <remarks>This class provides functionality to initialize, update, and manipulate the state of a harbor,
/// including operations such as dragging, rotating, and resetting vessels. It interacts with an <see
/// cref="IPortService"/> to load and manage port data. The state of the harbor is encapsulated in the <see
/// cref="Harbor"/> object, and changes to the state trigger the <see cref="StateChanged"/> event.</remarks>
/// <param name="portService"></param>
public class PortVM(IPortService portService) : IPortVM
{
    private readonly IPortService portService = portService;
    public IHarbor State { get; private set; } = Harbor.Default;
    public bool HasData => State != Harbor.Default;

    public event Action? StateChanged;

    public async Task<PortDto> InitializeAsync(PortDto? portDto = null)
    {
        if (portDto != null)
        {
            State = State = Harbor.Create(portDto);
            return portDto;
        }
        return await LoadData();
    }
    public async Task UpdateStateAsync(DomRectDto anchorageRect)
    {
        if (!HasData) await LoadData();
        State.UpdateAnchorage(anchorageRect);
    }
    public void StartDrag(Vessel v, DimDto client, DimDto offset)
    {
        State.Drag(v, client.X, client.Y, offset.X, offset.Y);
        OnStateChanged();
    }
    public void OnDrag(DimDto client)
    {
        State.DragGhost(client.X, client.Y);
        OnStateChanged();
    }

    public void EndDrag(DimDto client)
    {
        State.Drop(client.X, client.Y);
        OnStateChanged();
    }
    public void Rotate(Vessel v)
    {
        State.Rotate(v);
        OnStateChanged();
    }
    public void Reset()
    {
        State.Reset();
        OnStateChanged();
    }
    public async Task New()
    {
        await LoadData();
        OnStateChanged();
    }

    private async Task<PortDto> LoadData()
    {
        var portDto = await portService.GetPortDataAsync() ?? PortDto.Default;
        State = Harbor.Create(portDto);
        return portDto;
    }
    private void OnStateChanged() => StateChanged?.Invoke();
}