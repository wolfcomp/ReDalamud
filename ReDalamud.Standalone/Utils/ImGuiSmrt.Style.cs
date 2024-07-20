
// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault

namespace ReDalamud.Standalone.Utils;
public static partial class ImGuiSmrt
{
    public sealed class Style : IDisposable
    {
        internal static readonly List<(ImGuiStyleVar idx, object style)> Stack = new();

        private int _count;

        public static Type GetStyleIdxType(ImGuiStyleVar idx) => idx switch
        {
                ImGuiStyleVar.Alpha => typeof(float),
                ImGuiStyleVar.WindowRounding => typeof(float),
                ImGuiStyleVar.WindowBorderSize => typeof(float),
                ImGuiStyleVar.ChildRounding => typeof(float),
                ImGuiStyleVar.ChildBorderSize => typeof(float),
                ImGuiStyleVar.PopupRounding => typeof(float),
                ImGuiStyleVar.PopupBorderSize => typeof(float),
                ImGuiStyleVar.FrameRounding => typeof(float),
                ImGuiStyleVar.FrameBorderSize => typeof(float),
                ImGuiStyleVar.IndentSpacing => typeof(float),
                ImGuiStyleVar.ScrollbarSize => typeof(float),
                ImGuiStyleVar.ScrollbarRounding => typeof(float),
                ImGuiStyleVar.GrabMinSize => typeof(float),
                ImGuiStyleVar.GrabRounding => typeof(float),
                ImGuiStyleVar.TabRounding => typeof(float),
                ImGuiStyleVar.DisabledAlpha => typeof(float),
                ImGuiStyleVar.SeparatorTextBorderSize => typeof(float),
                ImGuiStyleVar.DockingSeparatorSize => typeof(float),
                ImGuiStyleVar.WindowPadding => typeof(Vector2),
                ImGuiStyleVar.WindowMinSize => typeof(Vector2),
                ImGuiStyleVar.WindowTitleAlign => typeof(Vector2),
                ImGuiStyleVar.FramePadding => typeof(Vector2),
                ImGuiStyleVar.ItemSpacing => typeof(Vector2),
                ImGuiStyleVar.ItemInnerSpacing => typeof(Vector2),
                ImGuiStyleVar.CellPadding => typeof(Vector2),
                ImGuiStyleVar.ButtonTextAlign => typeof(Vector2),
                ImGuiStyleVar.SelectableTextAlign => typeof(Vector2),
                ImGuiStyleVar.SeparatorTextAlign => typeof(Vector2),
                ImGuiStyleVar.SeparatorTextPadding => typeof(Vector2),
                _ => throw new ArgumentOutOfRangeException(nameof(idx), idx, null),
        };

        private static void CheckStyleIdx(ImGuiStyleVar idx, Type type)
        {
            if (GetStyleIdxType(idx) != type)
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
}
