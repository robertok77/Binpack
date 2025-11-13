namespace Binpack.UI.State.HarborDomain;

public class Anchorage
{
    public double AnchorageWidth { get; }
    public double AnchorageHeight { get; }
    public DomDimension? DomDimension { get; private set; }

    private Anchorage(double anchorageWidth, double anchorageHeight)
    {
        AnchorageWidth = anchorageWidth * Harbor.Multiplier;
        AnchorageHeight = anchorageHeight * Harbor.Multiplier;
    }
    internal static Anchorage Create(double anchorageWidth, double anchorageHeight) =>
        new(anchorageWidth, anchorageHeight);

    internal static Anchorage Default { get; } = new(0, 0);

    internal bool IsPlaced(Vessel vessel) => IsProposedPosition(vessel) && !vessel.OverlapsAny();
    internal void UpdateDimension(DomDimension domDimension) => DomDimension = domDimension;
    internal bool IsInsideAnchorage(double clientX, double clientY) => 
        DomDimension != null &&
        clientX >= DomDimension.Left && clientX <= DomDimension.Right &&
        clientY >= DomDimension.Top && clientY <= DomDimension.Bottom;
    internal bool CanInsideAnchorage(Vessel vessel) =>
        vessel.CurrentWidth <= AnchorageWidth &&
        vessel.CurrentHeight <= AnchorageHeight;

    private bool IsProposedPosition(Vessel vessel)
    {
        if (!vessel.State.InAnchorage && !CanInsideAnchorage(vessel)) return false;

        var x = Math.Max(0, Math.Min(vessel.State.X, AnchorageWidth - vessel.CurrentWidth));
        var y = Math.Max(0, Math.Min(vessel.State.Y, AnchorageHeight - vessel.CurrentHeight));
        vessel.SetState(x, y);
        return true;
    }
}