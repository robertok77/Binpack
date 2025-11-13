using Binpack.Shared.Models;
using Binpack.UI.State.HarborDomain;

namespace Binpack.UI.State.Abstractions;

public interface IHarbor
{
    Fleet Fleet { get; }
    Anchorage Anchorage { get; }
    DraggedVessel? DraggedVessel { get; }
    bool IsDragging { get; }
    bool Drag(Vessel vessel, double clientX, double clientY, double dragOffsetX, double dragOffsetY);
    bool Drop(double clientX, double clientY);
    void DragGhost(double clientX, double clientY);
    void Rotate(Vessel vessel);
    void Reset();
    void UpdateAnchorage(DomRectDto anchorageRect);
}