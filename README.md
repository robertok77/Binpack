# Binpack

This document explains how to run and interact with the Blazor Binpack in this repository and how to exercise the main UI behaviors (drag/drop, rotate, reset, load new data).

## Quick overview
- The Port UI is a Blazor page implemented by `src/Binpack.Web/Components/Pages/Port.razor` and `Port.razor.cs`.
- The page uses the `IPortVM` view model (`src/Binpack.Web/ViewModel/PortVM.cs`) to manage application state (`Harbor`) and user interactions.
- Ship layout and anchorage logic live in the `Binpack.UI.State` project (types such as `Harbor`, `Anchorage`, `Vessel`, `DraggedVessel`).

## Prerequisites
- .NET 9 SDK installed.
- Visual Studio 2022 (or later) for development; any editor that supports .NET can run the app.
- Browser with JavaScript enabled (the page fetches DOM bounding rectangles via JS interop).


## What the Port page does
- Loads port data (via `IPortService`) into the `PortVM` (`InitializeAsync`).
- On render it uses JS interop to get the browser `getBoundingClientRect` of the anchorage element and calls `UpdateStateAsync` to update anchorage DOM dimensions.
- Allows the user to drag ships into the anchorage area, move the ghost preview, and place ships by dropping.
- Provides rotate, reset and fetch-new-data operations from the UI. The view model raises `StateChanged` to notify the component to re-render.

## How to interact with the UI
- Initial load: the page either initializes with persisted state or requests data from the `IPortService`.
- Drag a ship:
  - Click (pointer down) on a ship to begin drag. The view model calls `StartDrag(vessel, client, offset)`.
  - Move the pointer — the view model calls `OnDrag(client)` and `DragGhost` updates the ghost position.
  - Release (pointer up) inside anchorage — the view model calls `EndDrag(client)` which attempts to place the ship.
- Rotate a ship: use the UI control that calls `Rotate(vessel)` (the view model decides whether the rotation is valid given placement/overlap).
- Reset: calls `Reset()` to clear placements and close any active drag.
- New: calls `New()` which fetches new port data and updates state.


## Files to inspect
- `src/Binpack.Web/Components/Pages/Port.razor` and `Port.razor.cs` — Blazor component and lifecycle hooks
- `src/Binpack.Web/ViewModel/PortVM.cs` — view model implementing `IPortVM`
- `src/Binpack.UI.State/HarborDomain/*` — core domain: `Harbor`, `Anchorage`, `Vessel`, `DraggedVessel`
- Tests:
  - `tests/Binpack.Web.Tests/PortVMTests.cs`
  - `tests/Binpack.UI.State.Tests/*`
