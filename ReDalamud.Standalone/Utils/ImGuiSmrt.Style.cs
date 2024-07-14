
// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault

namespace ReDalamud.Standalone.Utils;
public static partial class ImGuiSmrt
{
    public sealed class Style : IDisposable
    {
        internal static readonly List<(ImGuiStyleVar idx, object style)> Stack = new();

        private int _count;

        private static void CheckStyleIdx(ImGuiStyleVar idx, Type type)
        {
            var shouldThrow = idx switch
            {
                ImGuiStyleVar.Alpha => type != typeof(float),
                ImGuiStyleVar.WindowPadding => type != typeof(Vector2),
                ImGuiStyleVar.WindowRounding => type != typeof(float),
                ImGuiStyleVar.WindowBorderSize => type != typeof(float),
                ImGuiStyleVar.WindowMinSize => type != typeof(Vector2),
                ImGuiStyleVar.WindowTitleAlign => type != typeof(Vector2),
                ImGuiStyleVar.ChildRounding => type != typeof(float),
                ImGuiStyleVar.ChildBorderSize => type != typeof(float),
                ImGuiStyleVar.PopupRounding => type != typeof(float),
                ImGuiStyleVar.PopupBorderSize => type != typeof(float),
                ImGuiStyleVar.FramePadding => type != typeof(Vector2),
                ImGuiStyleVar.FrameRounding => type != typeof(float),
                ImGuiStyleVar.FrameBorderSize => type != typeof(float),
                ImGuiStyleVar.ItemSpacing => type != typeof(Vector2),
                ImGuiStyleVar.ItemInnerSpacing => type != typeof(Vector2),
                ImGuiStyleVar.IndentSpacing => type != typeof(float),
                ImGuiStyleVar.CellPadding => type != typeof(Vector2),
                ImGuiStyleVar.ScrollbarSize => type != typeof(float),
                ImGuiStyleVar.ScrollbarRounding => type != typeof(float),
                ImGuiStyleVar.GrabMinSize => type != typeof(float),
                ImGuiStyleVar.GrabRounding => type != typeof(float),
                ImGuiStyleVar.TabRounding => type != typeof(float),
                ImGuiStyleVar.ButtonTextAlign => type != typeof(Vector2),
                ImGuiStyleVar.SelectableTextAlign => type != typeof(Vector2),
                ImGuiStyleVar.DisabledAlpha => type != typeof(float),
                ImGuiStyleVar.SeparatorTextBorderSize => type != typeof(float),
                ImGuiStyleVar.SeparatorTextAlign => type != typeof(Vector2),
                ImGuiStyleVar.SeparatorTextPadding => type != typeof(Vector2),
                ImGuiStyleVar.DockingSeparatorSize => type != typeof(float),
                _ => throw new ArgumentOutOfRangeException(nameof(idx), idx, null),
            };

            if (shouldThrow)
                throw new ArgumentException($"Invalid type for {idx}");
        }

        private static float GetStyleFloat(ImGuiStyleVar idx)
        {
            var style = ImGui.GetStyle();
            return idx switch
            {
                ImGuiStyleVar.Alpha => style.Alpha,
                ImGuiStyleVar.WindowRounding => style.WindowRounding,
                ImGuiStyleVar.WindowBorderSize => style.WindowBorderSize,
                ImGuiStyleVar.ChildRounding => style.ChildRounding,
                ImGuiStyleVar.ChildBorderSize => style.ChildBorderSize,
                ImGuiStyleVar.PopupRounding => style.PopupRounding,
                ImGuiStyleVar.PopupBorderSize => style.PopupBorderSize,
                ImGuiStyleVar.FrameRounding => style.FrameRounding,
                ImGuiStyleVar.FrameBorderSize => style.FrameBorderSize,
                ImGuiStyleVar.IndentSpacing => style.IndentSpacing,
                ImGuiStyleVar.ScrollbarSize => style.ScrollbarSize,
                ImGuiStyleVar.ScrollbarRounding => style.ScrollbarRounding,
                ImGuiStyleVar.GrabMinSize => style.GrabMinSize,
                ImGuiStyleVar.GrabRounding => style.GrabRounding,
                ImGuiStyleVar.TabRounding => style.TabRounding,
                ImGuiStyleVar.DisabledAlpha => style.DisabledAlpha,
                ImGuiStyleVar.SeparatorTextBorderSize => style.SeparatorTextBorderSize,
                ImGuiStyleVar.DockingSeparatorSize => style.DockingSeparatorSize,
                _ => throw new ArgumentOutOfRangeException(nameof(idx), idx, null),
            };
        }

        private static Vector2 GetStyleVector2(ImGuiStyleVar idx)
        {
            var style = ImGui.GetStyle();
            return idx switch
            {
                ImGuiStyleVar.WindowPadding => style.WindowPadding,
                ImGuiStyleVar.WindowMinSize => style.WindowMinSize,
                ImGuiStyleVar.WindowTitleAlign => style.WindowTitleAlign,
                ImGuiStyleVar.ItemSpacing => style.ItemSpacing,
                ImGuiStyleVar.ItemInnerSpacing => style.ItemInnerSpacing,
                ImGuiStyleVar.FramePadding => style.FramePadding,
                ImGuiStyleVar.CellPadding => style.CellPadding,
                ImGuiStyleVar.ButtonTextAlign => style.ButtonTextAlign,
                ImGuiStyleVar.SelectableTextAlign => style.SelectableTextAlign,
                ImGuiStyleVar.SeparatorTextAlign => style.SeparatorTextAlign,
                ImGuiStyleVar.SeparatorTextPadding => style.SeparatorTextPadding,
                _ => throw new ArgumentOutOfRangeException(nameof(idx), idx, null),
            };
        }

        public Style Push(ImGuiStyleVar idx, float value)
        {
            CheckStyleIdx(idx, value.GetType());
            Stack.Add((idx, value));
            _count++;
            return this;
        }

        public Style Push(ImGuiStyleVar idx, Vector2 value)
        {
            CheckStyleIdx(idx, value.GetType());
            Stack.Add((idx, value));
            _count++;
            return this;
        }

        public void Pop(int num = 1)
        {
            num = Math.Min(num, _count);
            _count -= num;
            ImGui.PopStyleVar(num);
            Stack.RemoveRange(Stack.Count - num, num);
        }

        public void Dispose() => Pop(_count);
    }

    public class Color : IDisposable
    {
        public static List<(ImGuiCol idx, Standalone.Color color)> Stack = new();

        private int _count;

        public Color Push(ImGuiCol idx, Standalone.Color color)
        {
            Stack.Add((idx, color));
            ImGui.PushStyleColor(idx, (Vector4)color);
            _count++;
            return this;
        }

        public Color PushTextColor(Standalone.Color color) => Push(ImGuiCol.Text, color);

        public void Pop(int num = 1)
        {
            num = Math.Min(num, _count);
            _count -= num;
            ImGui.PopStyleColor(num);
            Stack.RemoveRange(Stack.Count - num, num);
        }

        public void Dispose() => Pop(_count);
    }
}
