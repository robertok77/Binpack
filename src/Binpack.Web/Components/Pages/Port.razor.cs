using Binpack.Shared.Models;
using Binpack.Web.Abstractions;
using Binpack.Web.Extensions;
using Binpack.Web.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Binpack.Web.Components.Pages;

public sealed partial class Port : IDisposable
{
    private ElementReference anchorageRef;
    private PersistingComponentStateSubscription? subscription;
    private PortDto? stateDto;
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private PersistentComponentState PersistentState { get; set; } = null!;
    [Inject] private IPortVM ViewModel { get; set; } = null!;

    protected override void OnInitialized() =>
        subscription = PersistentState.RegisterOnPersisting(PersistState);

    protected override async Task OnInitializedAsync()
    {
        ViewModel.StateChanged += StateChanged;
        PersistentState.TryTakeFromJson<PortDto>(nameof(Port), out stateDto);
        stateDto = await ViewModel.InitializeAsync(stateDto);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ViewModel.HasData)
        {
            var anchorageRect = await JS.GetBoundingClientRect(anchorageRef);
            await ViewModel.UpdateStateAsync(anchorageRect);
        }
        if (firstRender) StateHasChanged();

    }
    private Task PersistState()
    {
        if (stateDto != null)
        {
            PersistentState.PersistAsJson(nameof(Port), stateDto);
        }
        return Task.CompletedTask;
    }
    private void StateChanged() => StateHasChanged();

    public void Dispose()
    {
        ViewModel.StateChanged -= StateChanged;
        subscription?.Dispose();
    }
}