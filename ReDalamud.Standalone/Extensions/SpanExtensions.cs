namespace ReDalamud.Standalone.Extensions;
public static class SpanExtensions
{
    public static ReadOnlySpan<T> Reverse<T>(this ReadOnlySpan<T> span)
    {
        var copy = span.ToArray();
        copy.AsSpan().Reverse();
        return copy;
    }
}
