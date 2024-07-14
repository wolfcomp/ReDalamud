using Vector4 = System.Numerics.Vector4;
using ExtColor = ReDalamud.Standalone.Color;

namespace ReDalamud.Standalone.Utils;
public static partial class ImGuiSmrt
{
    public static Style PushStyle(ImGuiStyleVar idx, float value) => new Style().Push(idx, value);
    public static Style PushStyle(ImGuiStyleVar idx, Vector2 value) => new Style().Push(idx, value);

    public static Style PushDefaultStyle()
    {
        var ret = new Style();
        var stack = Style.Stack.GroupBy(p => p.idx).Select(p => (p.Key, p.First().style)).ToArray();
        foreach (var (idx, style) in stack)
        {
            switch (style)
            {
                case float f:
                    ret.Push(idx, f);
                    break;
                case Vector2 v:
                    ret.Push(idx, v);
                    break;
            }
        }
        return ret;
    }

    public static Color PushColor(ImGuiCol idx, ExtColor color) => new Color().Push(idx, color);
    public static Color PushTextColor(ExtColor color) => new Color().PushTextColor(color);

    public static Color PushDefaultColor()
    {
        var ret = new Color();
        var stack = Color.Stack.GroupBy(p => p.idx).Select(p => (p.Key, p.First().color)).ToArray();
        foreach (var (idx, color) in stack)
        {
            ret.Push(idx, color);
        }
        return ret;
    }
}
