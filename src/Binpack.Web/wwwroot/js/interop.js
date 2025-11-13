function getBoundingClientRect(element) {
    if (!element) return { Left: 0, Top: 0, Right: 0, Bottom: 0, Width: 0, Height: 0 };
    const r = element.getBoundingClientRect();
    return { Left: r.left, Top: r.top, Right: r.right, Bottom: r.bottom, Width: r.width, Height: r.height };
}