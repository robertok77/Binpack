using Binpack.Infrastructure.Abstractions;
using Binpack.Shared.Models;
using Binpack.UI.State.HarborDomain;
using Binpack.Web.ViewModel;
using NSubstitute;
using Shouldly;

namespace Binpack.Web.Tests;

public class PortVMTests
{
    [Fact]
    public async Task InitializeAsync_WithPortDto_SetsStateAndReturnsDto()
    {
        var svc = Substitute.For<IPortService>();
        var vm = new PortVM(svc);

        var dto = new PortDto(new AnchorageSizeDto(5, 5), []);

        var result = await vm.InitializeAsync(dto);

        result.ShouldBe(dto);
        vm.State.ShouldNotBe(Harbor.Default);
        vm.HasData.ShouldBeTrue();
    }

    [Fact]
    public async Task InitializeAsync_NoArg_CallsServiceAndSetsState()
    {
        var svc = Substitute.For<IPortService>();
        var dto = new PortDto(new AnchorageSizeDto(3, 4), []);
        svc.GetPortDataAsync().Returns(Task.FromResult<PortDto?>(dto));

        var vm = new PortVM(svc);

        var result = await vm.InitializeAsync();

        await svc.Received(1).GetPortDataAsync();
        result.ShouldBe(dto);
        vm.HasData.ShouldBeTrue();
        vm.State.Anchorage.AnchorageWidth.ShouldBe(dto.AnchorageSize.Width * Harbor.Multiplier);
    }

    [Fact]
    public async Task UpdateStateAsync_WhenNoData_LoadsDataAndUpdatesAnchorage()
    {
        var svc = Substitute.For<IPortService>();
        var dto = new PortDto(new AnchorageSizeDto(6, 7), []);
        svc.GetPortDataAsync().Returns(Task.FromResult<PortDto?>(dto));

        var vm = new PortVM(svc);
        vm.HasData.ShouldBeFalse();

        var rect = new DomRectDto() { Left = 1, Top = 2, Right = 61, Bottom = 72, Width = 60, Height = 70 };
        await vm.UpdateStateAsync(rect);

        await svc.Received(1).GetPortDataAsync();
        vm.State.Anchorage.DomDimension.ShouldNotBeNull();
        vm.State.Anchorage.IsInsideAnchorage(rect.Left + 5, rect.Top + 5).ShouldBeTrue();
    }

    [Fact]
    public async Task StartDrag_OnDrag_EndDrag_RaiseStateChanged_And_ChangeHarborState()
    {
        var svc = Substitute.For<IPortService>();
        svc.GetPortDataAsync().Returns(Task.FromResult<PortDto?>(PortDto.Default));

        var vm = new PortVM(svc);
        await vm.InitializeAsync();

        var calls = 0;
        vm.StateChanged += () => calls++;

        var vessel = Vessel.CreateBuilder("V", 2, 2, "D")
            .WithFleet(Fleet.Create([]))
            .Build();

        var client = new DimDto(10, 10);
        var offset = new DimDto(1, 1);

        vm.StartDrag(vessel, client, offset);
        calls.ShouldBeGreaterThan(0);
        vm.State.IsDragging.ShouldBeTrue();

        var beforeCalls = calls;
        vm.OnDrag(new DimDto(15, 16));
        calls.ShouldBeGreaterThan(beforeCalls);
        vm.State.DraggedVessel.ShouldNotBeNull();

        vm.EndDrag(new DimDto(15, 16));

        vm.State.DraggedVessel.ShouldBeNull();
        calls.ShouldBeGreaterThan(beforeCalls);
    }

    [Fact]
    public async Task Rotate_Reset_New_InvokeStateChanged()
    {
        var svc = Substitute.For<IPortService>();
        svc.GetPortDataAsync().Returns(Task.FromResult<PortDto?>(PortDto.Default));

        var vm = new PortVM(svc);
        await vm.InitializeAsync();

        var calls = 0;
        vm.StateChanged += () => calls++;

        var vessel = vm.State.Fleet.Vessels.Count > 0
            ? (vm.State.Fleet.Vessels.First())
            : Vessel.CreateBuilder("X", 1, 1, "D").WithFleet(Fleet.Create([])).Build();

        var initialRotated = vessel.State.IsRotated;
        vm.Rotate(vessel);
        calls.ShouldBeGreaterThan(0);
        vessel.State.IsRotated.ShouldBe(!initialRotated);

        var callsBeforeReset = calls;
        vm.Reset();
        calls.ShouldBeGreaterThan(callsBeforeReset);

        var callsBeforeNew = calls;
        await vm.New();
        calls.ShouldBeGreaterThan(callsBeforeNew);
        await svc.Received().GetPortDataAsync();
    }
}