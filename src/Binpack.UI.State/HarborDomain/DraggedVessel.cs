namespace Binpack.UI.State.HarborDomain;

public class DraggedVessel
{
    public Anchorage Anchorage { get; }
    public Vessel Vessel { get; }
    public double OffsetX { get; }
    public double OffsetY { get; }
    public double GhostLeft { get; private set; }
    public double GhostTop { get; private set; }

    private VesselSnapshot? snapshot;

    private DraggedVessel(Anchorage anchorage,Vessel vessel, double offsetX, double offsetY)
    {
        Anchorage = anchorage;
        Vessel = vessel;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }
    internal static DraggedVessel Create(Anchorage anchorage, Vessel vessel, double offsetX, double offsetY)
    {
        if (offsetX < 0) throw new ArgumentOutOfRangeException(nameof(offsetX));
        if (offsetY < 0) throw new ArgumentOutOfRangeException(nameof(offsetY));
        return new DraggedVessel(anchorage,vessel, offsetX, offsetY);
    }
    internal static DraggedVessel Default { get; } = new(Anchorage.Default, Vessel.Default, 0, 0);
    internal DraggedVessel Initialize()
    {
        snapshot = Vessel.GetStateSnapshot;
        Vessel.SetState(isDragging: true);
        return this;
    }
    internal DraggedVessel DragGhost(double clientX, double clientY)
    {
        GhostLeft = clientX - OffsetX;
        GhostTop = clientY - OffsetY;
        return this;
    }
    internal void Place(double clientX, double clientY)
    {
        if (!(Anchorage.CanInsideAnchorage(Vessel) && Anchorage.IsInsideAnchorage(clientX, clientY))) 
            return;

        var proposed = ProposedCoordinates(clientX, clientY);
        Vessel.SetState(true, proposed.X, proposed.Y);
        if (Vessel.OverlapsAny())
        {
            Revert();
        }
    }

    internal void Revert() => snapshot?.Revert();
    internal void Close() => Vessel.SetState(isDragging: false);

    internal (double X, double Y) ProposedCoordinates(double clientX, double clientY)
    {
        if(Anchorage.DomDimension==null) throw new InvalidOperationException("Anchorage DomDimension is not set.");
        var proposedX = clientX - Anchorage.DomDimension.Left - OffsetX;
        var proposedY = clientY - Anchorage.DomDimension.Top - OffsetY;

        proposedX = Math.Max(0, Math.Min(proposedX, Anchorage.AnchorageWidth - Vessel.CurrentWidth));
        proposedY = Math.Max(0, Math.Min(proposedY, Anchorage.AnchorageHeight - Vessel.CurrentHeight));
        return (proposedX, proposedY);
    }
}