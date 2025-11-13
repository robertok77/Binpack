namespace Binpack.UI.State.HarborDomain;

public partial class Vessel
{
    public string Name { get; }
    public double Width { get; }
    public double Height { get; }
    public string ShipDesignation { get; }

    public double CurrentWidth => State.IsRotated ? Height : Width;
    public double CurrentHeight => State.IsRotated ? Width : Height;
    public VesselState State { get; private set; } = new();
    internal VesselState CloneState => State with { };

    private Fleet _fleet = Fleet.Default;

    internal VesselSnapshot GetStateSnapshot => new(this);
    
    private Vessel(string name, double width, double height, string shipDesignation)
    {
        Name = name;
        Width = width * Harbor.Multiplier;
        Height = height * Harbor.Multiplier;
        ShipDesignation = shipDesignation;
    }

    internal static VesselBuilder CreateBuilder(string name, double width, double height, string shipDesignation) => new(name, width, height, shipDesignation);
    internal static Vessel Default { get; } = new( string.Empty, 0, 0, string.Empty);

    public string DisplayName => $"{Name} ({(int)CurrentWidth}×{(int)CurrentHeight})";

    internal void Rotate() => State = State with { IsRotated = !State.IsRotated };
    internal void Rotate(Func<bool> isInPlaced)
    {
        var snapshot = GetStateSnapshot;
        Rotate();
        if (!isInPlaced()) 
            snapshot.Revert();
    }
    internal void ResetState() => State = new();
    internal bool OverlapsAny() => _fleet.OverlapsAny(this);
    internal void SetState(double x, double y) => State = State with { X = x, Y = y };
    internal void SetState(VesselState newState) => State = newState with { };
    internal void SetState(bool inAnchorage, double x, double y) => State = State with { InAnchorage = inAnchorage, X = x, Y = y };
    internal void SetState(bool isDragging) => State = State with { IsDragging = isDragging };
}