using Binpack.Shared.Models;

namespace Binpack.UI.State.HarborDomain;

public record DomDimension
{
    public double Left { get; }
    public double Top { get; }
    public double Right { get; }
    public double Bottom { get; }
    public double Width { get; }
    public double Height { get; }
    internal DomDimension(DomRectDto domRect)
    {
        Left = domRect.Left >= 0 ? Math.Ceiling(domRect.Left) : Math.Ceiling(domRect.Right - domRect.Width);
        Top = domRect.Top >= 0 ? Math.Ceiling(domRect.Top) : Math.Ceiling(domRect.Bottom - domRect.Height);
        Right = domRect.Right >= 0 ? Math.Floor(domRect.Right) : Math.Floor(domRect.Left + domRect.Width);
        Bottom = domRect.Bottom >= 0 ? Math.Floor(domRect.Bottom) : Math.Floor(domRect.Top + domRect.Height);
        Width = domRect.Width;
        Height = domRect.Height;
    }
}