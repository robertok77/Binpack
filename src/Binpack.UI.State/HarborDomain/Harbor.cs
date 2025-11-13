using Binpack.Shared.Models;
using Binpack.UI.State.Abstractions;

namespace Binpack.UI.State.HarborDomain;
/// <summary>
/// State for Port ViewModel. All operations for manipulating vessels and anchorage are done through this class.
/// 
/// Represents a harbor that manages a fleet of vessels, an anchorage area, and the ability to drag and drop vessels.
/// </summary>
/// <remarks>The <see cref="Harbor"/> class provides functionality to manage a fleet of vessels and an anchorage
/// area,  as well as operations for dragging, dropping, and rotating vessels. It also supports resetting the state  of
/// the harbor and updating the dimensions of the anchorage.</remarks>
public class Harbor : IHarbor
{
    public Fleet Fleet { get; }
    public Anchorage Anchorage { get; private set; }
    public DraggedVessel? DraggedVessel { get; private set; }

    public bool IsDragging => DraggedVessel is not null;

    public const int Multiplier = 10;
    
    private Harbor(Fleet fleet, Anchorage anchorage)
    {
        Fleet = fleet;
        Anchorage = anchorage;
    }

    public static IHarbor Create(PortDto portDto)
    {
        var anchorage = Anchorage.Create(portDto.AnchorageSize.Width, portDto.AnchorageSize.Height);

        var fleet = Fleet.Create(portDto.Fleets);

        return new Harbor(fleet, anchorage);
    }
    public static IHarbor Default { get; } = new Harbor(Fleet.Default,Anchorage.Default);
    public bool Drag(Vessel vessel, double clientX, double clientY, double dragOffsetX, double dragOffsetY)
    {
        if (clientX < 0 || clientY < 0 || dragOffsetX < 0 || dragOffsetY < 0) return false;

        DraggedVessel = DraggedVessel
            .Create(Anchorage, vessel, dragOffsetX, dragOffsetY)
            .Initialize()
            .DragGhost(clientX, clientY);
        return true;
    }
    public bool Drop(double clientX, double clientY)
    {
        var result = clientX >= 0 && clientY >= 0;
        try
        {
            if (result) DraggedVessel?.Place(clientX, clientY);
        }
        finally
        {
            DraggedVessel?.Close();
            DraggedVessel = null;
        }
        return result;
    }
    public void DragGhost(double clientX, double clientY)
    {
        if (!IsDragging) return;
        DraggedVessel?.DragGhost(clientX, clientY);
    }
    public void Rotate(Vessel vessel) => vessel.Rotate(() => 
        !vessel.State.InAnchorage || Anchorage.IsPlaced(vessel));
    public void Reset()
    {
        Fleet.Reset();
        DraggedVessel?.Close();
        DraggedVessel = null;
    }
    public void UpdateAnchorage(DomRectDto anchorageRect) => Anchorage.UpdateDimension(new DomDimension(anchorageRect));
}