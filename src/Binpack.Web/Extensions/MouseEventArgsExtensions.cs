using Binpack.Shared.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Binpack.Web.Extensions;

public static class MouseEventArgsExtensions
{
    public static DimDto GetClient(this MouseEventArgs e) => new(e.ClientX, e.ClientY);
    public static DimDto GetOffset(this MouseEventArgs e) => new(e.OffsetX, e.OffsetY);
}