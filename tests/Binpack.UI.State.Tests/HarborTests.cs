using Binpack.Shared.Models;
using Binpack.UI.State.HarborDomain;
using Shouldly;

namespace Binpack.UI.State.Tests;

public class HarborTests
{
    [Fact]
    public void Create_FromPortDto_InitializesFleetAndAnchorage()
    {
        var port = PortDto.Default;
        var harbor = Harbor.Create(port);

        harbor.Anchorage.AnchorageWidth.ShouldBe(port.AnchorageSize.Width * Harbor.Multiplier);
        harbor.Anchorage.AnchorageHeight.ShouldBe(port.AnchorageSize.Height * Harbor.Multiplier);

        harbor.Fleet.Vessels.Count.ShouldBe(port.Fleets.Select(f => f.ShipCount).Sum());
    }

    [Fact]
    public void Drag_ReturnsFalse_ForNegativeInputs()
    {
        var harbor = Harbor.Default;
        var vessel = Vessel.CreateBuilder("V", 1, 1, "D")
            .WithFleet(Fleet.Create([]))
            .Build();

        harbor.Drag(vessel, -1, 0, 1, 1).ShouldBeFalse();
        harbor.Drag(vessel, 1, -1, 1, 1).ShouldBeFalse();
        harbor.Drag(vessel, 1, 1, -1, 1).ShouldBeFalse();
        harbor.Drag(vessel, 1, 1, 1, -1).ShouldBeFalse();

        harbor.IsDragging.ShouldBeFalse();
        harbor.DraggedVessel.ShouldBeNull();
    }

    [Fact]
    public void Drag_SetsDraggedVessel_IsDragging_And_SetsGhost()
    {
        var port = new PortDto(new AnchorageSizeDto(5, 5), []);
        var harbor = Harbor.Create(port);

        var vessel = Vessel.CreateBuilder("V", 2, 2, "D")
            .WithFleet(Fleet.Create([]))
            .Build();

        var clientX = 30.0;
        var clientY = 40.0;
        var offsetX = 5.0;
        var offsetY = 6.0;

        var result = harbor.Drag(vessel, clientX, clientY, offsetX, offsetY);

        result.ShouldBeTrue();
        harbor.IsDragging.ShouldBeTrue();
        harbor.DraggedVessel.ShouldNotBeNull();
        harbor.DraggedVessel?.GhostLeft.ShouldBe(clientX - offsetX);
        harbor.DraggedVessel?.GhostTop.ShouldBe(clientY - offsetY);

        vessel.State.IsDragging.ShouldBeTrue();
    }

    [Fact]
    public void Drop_WithPositiveCoordinates_PlacesVessel_WhenInsideAnchorage()
    {
        var port = new PortDto(new AnchorageSizeDto(5, 5), []); 
        var harbor = Harbor.Create(port);

        var rect = new DomRectDto() { Left = 0, Top = 0, Right = 50, Bottom = 50, Width = 50, Height = 50 };
        harbor.UpdateAnchorage(rect);

        var vessel = Vessel.CreateBuilder("V", 3, 3, "D")
            .WithFleet(Fleet.Create([]))
            .Build();

        var clientX = 20.0;
        var clientY = 20.0;
        var offsetX = 2.0;
        var offsetY = 2.0;
        harbor.Drag(vessel, clientX, clientY, offsetX, offsetY).ShouldBeTrue();

        var dropped = harbor.Drop(clientX, clientY);
        dropped.ShouldBeTrue();

        harbor.DraggedVessel.ShouldBeNull();
        vessel.State.IsDragging.ShouldBeFalse();

        vessel.State.InAnchorage.ShouldBeTrue();

        var expectedX = Math.Max(0, Math.Min(clientX - rect.Left - offsetX, harbor.Anchorage.AnchorageWidth - vessel.CurrentWidth));
        var expectedY = Math.Max(0, Math.Min(clientY - rect.Top - offsetY, harbor.Anchorage.AnchorageHeight - vessel.CurrentHeight));
        vessel.State.X.ShouldBe(expectedX);
        vessel.State.Y.ShouldBe(expectedY);
    }

    [Fact]
    public void Drop_WithNegativeCoordinates_ReturnsFalse_And_ClearsDraggedVessel()
    {
        var port = new PortDto(new AnchorageSizeDto(5, 5), []);
        var harbor = Harbor.Create(port);

        var vessel = Vessel.CreateBuilder("V", 1, 1, "D")
            .WithFleet(Fleet.Create([]))
            .Build();

        harbor.Drag(vessel, 10, 10, 1, 1).ShouldBeTrue();

        var result = harbor.Drop(-1, 0);
        result.ShouldBeFalse();

        harbor.DraggedVessel.ShouldBeNull();
        vessel.State.IsDragging.ShouldBeFalse();
    }

    [Fact]
    public void DragGhost_DoesNothing_WhenNotDragging_And_UpdatesGhost_WhenDragging()
    {
        var harbor = Harbor.Default;

        harbor.DragGhost(5, 6);

        var port = new PortDto(new AnchorageSizeDto(5, 5), []);
        harbor = Harbor.Create(port);

        var vessel = Vessel.CreateBuilder("V", 1, 1, "D")
            .WithFleet(Fleet.Create([]))
            .Build();

        harbor.Drag(vessel, 10, 10, 2, 2).ShouldBeTrue();
        harbor.DragGhost(15, 18);

        harbor.DraggedVessel.ShouldNotBeNull();
        harbor.DraggedVessel?.GhostLeft.ShouldBe(15 - 2);
        harbor.DraggedVessel?.GhostTop.ShouldBe(18 - 2);
    }

    [Fact]
    public void Reset_ClosesDraggedVessel_And_ResetsFleet()
    {
        var port = new PortDto(new AnchorageSizeDto(5, 5), []);
        var harbor = Harbor.Create(port);

        var vessel = Vessel.CreateBuilder("V", 1, 1, "D")
            .WithFleet(Fleet.Create([]))
            .Build();

        harbor.Drag(vessel, 10, 10, 1, 1).ShouldBeTrue();
        harbor.IsDragging.ShouldBeTrue();

        harbor.Reset();

        harbor.IsDragging.ShouldBeFalse();
        harbor.DraggedVessel.ShouldBeNull();
        vessel.State.IsDragging.ShouldBeFalse();
    }

    [Fact]
    public void UpdateAnchorage_SetsDomDimension_OnAnchorage()
    {
        var harbor = Harbor.Create(new PortDto(new AnchorageSizeDto(5, 5), []));
        var rect = new DomRectDto() { Left = 5, Top = 5, Right = 55, Bottom = 55, Width = 50, Height = 50 };

        harbor.UpdateAnchorage(rect);

        harbor.Anchorage.DomDimension.ShouldNotBeNull();
        harbor.Anchorage.IsInsideAnchorage(10, 10).ShouldBeTrue();
        harbor.Anchorage.IsInsideAnchorage(0, 0).ShouldBeFalse();
    }
}