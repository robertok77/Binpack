using Binpack.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Binpack.Web.Extensions;

internal static class JSInteropExtensions
{
    public static ValueTask<DomRectDto> GetBoundingClientRect(this IJSRuntime js, ElementReference element) =>
        js.InvokeAsync<DomRectDto>("getBoundingClientRect", element);
}