using Binpack.Shared.Models;
using Binpack.UI.State.HarborDomain;
using Shouldly;

namespace Binpack.UI.State.Tests;

public class AnchorageTests
{
    [Fact]
    public void Create_MultipliesDimensions_ByHarborMultiplier()
    {
        var anch = Anchorage.Create(5, 6);

        anch.AnchorageWidth.ShouldBe(5 * Harbor.Multiplier);
        anch.AnchorageHeight.ShouldBe(6 * Harbor.Multiplier);
    }

    [Fact]
    public void Default_IsZeroSize()
    {
        var anch = Anchorage.Default;

        anch.AnchorageWidth.ShouldBe(0);
        anch.AnchorageHeight.ShouldBe(0);
    }

    [Fact]
    public void CanInsideAnchorage_ReturnsTrue_WhenVesselFits()
    {
        var anch = Anchorage.Create(5, 5); 
        var vessel = Vessel.CreateBuilder("V", 4, 4, "D")
                           .WithFleet(Fleet.Create([]))
                           .Build(); 

        anch.CanInsideAnchorage(vessel).ShouldBeTrue();
    }

    [Fact]
    public void CanInsideAnchorage_ReturnsFalse_WhenVesselTooLarge()
    {
        var anch = Anchorage.Create(5, 5); 
        var vessel = Vessel.CreateBuilder("V", 6, 6, "D")
                           .WithFleet(Fleet.Create([]))
                           .Build();

        anch.CanInsideAnchorage(vessel).ShouldBeFalse();
    }

    [Fact]
    public void IsPlaced_SetsAndClampsState_WhenProposedPositionOutsideBounds_ButFits()
    {
        var anch = Anchorage.Create(5, 5); 
        var vessel = Vessel.CreateBuilder("V", 3, 3, "D")
                           .WithFleet(Fleet.Create([]))
                           .Build();

        vessel.SetState(inAnchorage: false, x: 1000, y: 2000);

        var placed = anch.IsPlaced(vessel);

        placed.ShouldBeTrue();

        vessel.State.X.ShouldBe(anch.AnchorageWidth - vessel.CurrentWidth);
        vessel.State.Y.ShouldBe(anch.AnchorageHeight - vessel.CurrentHeight);
    }

    [Fact]
    public void IsPlaced_ReturnsFalse_AndDoesNotChangeState_WhenVesselTooLargeAndNotInAnchorage()
    {
        var anch = Anchorage.Create(5, 5); 
        var vessel = Vessel.CreateBuilder("V", 6, 6, "D")
                           .WithFleet(Fleet.Create([]))
                           .Build(); 

        vessel.SetState(inAnchorage: false, x: 10, y: 11);

        var placed = anch.IsPlaced(vessel);

        placed.ShouldBeFalse();
        vessel.State.X.ShouldBe(10);
        vessel.State.Y.ShouldBe(11);
        vessel.State.InAnchorage.ShouldBeFalse();
    }

    [Fact]
    public void IsPlaced_AllowsOversize_WhenAlreadyInAnchorage_AndClampsPositionToZero()
    {
        var anch = Anchorage.Create(5, 5); 
        var vessel = Vessel.CreateBuilder("V", 6, 6, "D")
                           .WithFleet(Fleet.Create([]))
                           .Build(); 

        vessel.SetState(inAnchorage: true, x: 1000, y: 1000);

        var placed = anch.IsPlaced(vessel);

        placed.ShouldBeTrue();
        vessel.State.X.ShouldBe(0);
        vessel.State.Y.ShouldBe(0);
    }

    [Fact]
    public void UpdateDimension_SetsDomDimension_And_IsInsideAnchorage_Works()
    {
        var anch = Anchorage.Create(5, 5);

        var rect = new DomRectDto() { Left = 5, Top = 5, Right = 55, Bottom = 55, Width = 50, Height = 50 };
        var dim = new DomDimension(rect);

        anch.UpdateDimension(dim);

        anch.IsInsideAnchorage(10, 10).ShouldBeTrue();
        anch.IsInsideAnchorage(0, 0).ShouldBeFalse();
        anch.IsInsideAnchorage(60, 60).ShouldBeFalse();
    }
}