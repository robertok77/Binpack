namespace Binpack.UI.State.HarborDomain;

public record VesselState(
    bool InAnchorage = false,
    bool IsRotated = false,
    double X = 0,
    double Y = 0,
    bool IsDragging = false);