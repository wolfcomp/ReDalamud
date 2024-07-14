using System.Runtime.InteropServices;
using FFXIVClientStructs.Interop;
using System.Text;

namespace ReDalamud.Standalone.Utils;

public enum ImGuiDockNodeState
{
    Unknown,
    HostWindowHiddenBecauseSingleWindow,
    HostWindowHiddenBecauseWindowsAreResizing,
    HostWindowVisible
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ImGuiDockNode
{
    public uint ID;
    public ImGuiDockNodeFlags SharedFlags;
    public ImGuiDockNodeFlags LocalFlags;
    public ImGuiDockNodeFlags LocalFlagsInWindows;
    public ImGuiDockNodeFlags MergedFlags;
    public ImGuiDockNodeState State;
    public ImGuiDockNode* ParentNode;
}

public static class ImGuiExt
{
    #region DockBuilder
    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderDockWindow(byte* window_name, uint node_id);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe ImGuiDockNode* igDockBuilderGetNode(uint node_id);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe ImGuiDockNode* igDockBuilderGetCentralNode(uint node_id);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe uint igDockBuilderAddNode(uint node_id = 0, ImGuiDockNodeFlags flags = ImGuiDockNodeFlags.None);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderRemoveNode(uint node_id);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderRemoveNodeDockedWindows(uint node_id, bool clear_settings_refs = true);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderRemoveNodeChildNodes(uint node_id);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderSetNodePos(uint node_id, Vector2 pos);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderSetNodeSize(uint node_id, Vector2 size);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe uint igDockBuilderSplitNode(uint node_id, ImGuiDir split_dir, float size_ratio_for_node_at_dir, uint* out_id_at_dir, uint* out_id_at_opposite_dir);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderCopyDockSpace(uint src_dockspace_id, uint dst_dockspace_id, ImVector<Pointer<byte>>* window_remap_pairs);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderCopyNode(uint src_node, uint dst_node, ImVector<uint>* window_remap_pairs);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderCopyWindowSettings(byte* src_name, byte* dst_name);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igDockBuilderFinish(uint node_id);

    public static unsafe void DockBuilderDockWindow(string windowName, uint nodeId)
    {
        var bytes = Encoding.UTF8.GetBytes(windowName);
        fixed (byte* p = bytes)
        {
            igDockBuilderDockWindow(p, nodeId);
        }
    }
    #endregion

    #region Other
    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igItemSize_Rect(ImRect* rect, float text_baseline_y);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void igItemSize_Vector(Vector2 size, float text_baseline_y);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe bool igItemAdd(ImRect* rect, uint id, ImRect* nav_bb, ImGuiItemFlags extraFlags);
    public static unsafe bool igItemAdd(ImRect* rect, uint id) => igItemAdd(rect, id, null, ImGuiItemFlags.None);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe bool igBeginMenuEx(byte* label, byte* icon, bool enabled);
    #endregion

    private const float PI = float.Pi;
    private const float PI_2 = PI * 2;

    public static void PushTextStyleColor(Color color)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, (Vector4)color);
    }

    public static bool SelectableWithIcon(Icon16 icon, string label)
    {
        var style = ImGui.GetStyle();
        var pos = ImGui.GetCursorScreenPos();
        pos.X -= style.WindowPadding.X / 2;
        ImGui.SetCursorScreenPos(pos);
        var childSize = style.FramePadding;
        childSize.Y = ImGui.GetTextLineHeightWithSpacing();
        childSize.X += ImGui.CalcTextSize(label).X;
        childSize.X += 16;
        childSize.X += style.ItemSpacing.X * 2;
        var b = false;
        var hovered = ImGui.IsMouseHoveringRect(pos, pos + childSize);
        if (hovered)
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, style.Colors[(int)ImGuiCol.ButtonHovered]);
            b = true;
        }
        ImGui.BeginChild($"SelectableWithIcon##{label}", childSize, false, ImGuiWindowFlags.NoScrollbar);
        ImGui.Image(IconLoader.GetIconTextureId(icon), new Vector2(16, 16));
        ImGui.SameLine();
        ImGui.TextUnformatted(label);
        ImGui.SameLine();
        ImGui.EndChild();
        if (b)
            ImGui.PopStyleColor();
        return ImGui.IsItemHovered() && ImGui.IsItemClicked(ImGuiMouseButton.Left);
    }

    public static bool MenuWithIcon(Icon16 icon, string label, string popupLabel)
    {
        var style = ImGui.GetStyle();
        var pos = ImGui.GetCursorScreenPos();
        pos.X -= style.WindowPadding.X / 2;
        ImGui.SetCursorScreenPos(pos);
        var childSize = style.FramePadding;
        childSize.Y = ImGui.GetTextLineHeightWithSpacing();
        childSize.X += ImGui.CalcTextSize(label).X;
        childSize.X += 16;
        childSize.X += style.ItemSpacing.X * 2;
        childSize.X += ImGui.GetTextLineHeight() / 4;
        var b = false;
        var hovered = ImGui.IsMouseHoveringRect(pos, pos + childSize) || ImGui.IsPopupOpen(popupLabel);
        if (hovered)
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, style.Colors[(int)ImGuiCol.ButtonHovered]);
            b = true;
        }
        ImGui.BeginChild($"MenuWithIcon##{label}", childSize, false, ImGuiWindowFlags.NoScrollbar);
        ImGui.Image(IconLoader.GetIconTextureId(icon), new Vector2(16, 16));
        ImGui.SameLine();
        ImGui.TextUnformatted(label);
        ImGui.SameLine();
        pos = ImGui.GetCursorScreenPos();
        var lineHeight = ImGui.GetTextLineHeight() / 2;
        pos.Y += lineHeight / 2;
        var size = new Vector2(lineHeight);
        ImGui.Dummy(size);
        var drawList = ImGui.GetWindowDrawList();
        drawList.AddTriangleFilled(pos, pos + new Vector2(0, lineHeight), pos + size / 2, Color.FromRGBFloat(1, 1, 1));
        pos.X += size.X / 2;
        pos.X += style.ItemSpacing.X / 2;
        pos.Y -= lineHeight / 2;
        ImGui.EndChild();
        if (b)
            ImGui.PopStyleColor();
        if (hovered)
        {
            drawList.AddRectFilled(pos, pos + childSize with { X = style.WindowPadding.X / 2 }, ImGui.GetColorU32(ImGuiCol.ButtonHovered));
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                ImGui.OpenPopup(popupLabel);
        }
        var isOpen = ImGui.IsPopupOpen(popupLabel);
        if (isOpen)
            ImGui.SetNextWindowPos(pos + childSize with { X = style.WindowPadding.X / 2, Y = 0 });
        return isOpen && ImGui.Begin(popupLabel, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
    }

    public static unsafe void SpinnerDots(string label, ref float nextdot, float radius, float thickness, Color color, float speed = 2f,
        int dots = 12, float minth = -1f)
    {
        #region SpinnerHeader
        Vector2 pos, size, centre;
        int numSegments;
        var drawList = ImGui.GetWindowDrawList();
        var style = ImGui.GetStyle();
        pos = ImGui.GetCursorScreenPos();
        size = new Vector2(radius * 2, (radius + style.FramePadding.Y) * 2);
        var bb = new ImRect(pos, pos + size);
        igItemSize_Rect(&bb, style.FramePadding.Y);
        numSegments = drawList._CalcCircleAutoSegmentCount(radius);
        centre = bb.Center;
        var id = ImGui.GetID(label);
        if (!igItemAdd(&bb, id))
            return;
        #endregion

        var start = (float)ImGui.GetTime() * speed;
        float bgAngleOffset = PI_2 / dots;
        dots = int.Min(dots, 32);
        var mdots = dots / 2;

        if (nextdot < 0)
            nextdot = dots;

        for (var i = 0; i <= dots; i++)
        {
            var a = (start + (i * bgAngleOffset)) % PI_2;
            var th = minth < 0 ? thickness / 2 : minth;
            if (nextdot + mdots < dots)
            {
                if (i > nextdot && i < nextdot + mdots)
                    th = thcorrect(nextdot, i);
            }
            else
            {
                if ((i > nextdot && i < dots) || i < (int)(nextdot + mdots) % dots)
                    th = thcorrect(nextdot, i);
            }
            drawList.AddCircleFilled(new Vector2(centre.X + float.Cos(-a) * radius, centre.Y + float.Sin(-a) * radius), th, color, 8);
        }

        return;

        float thcorrect(float nextdot, int i)
        {
            var nth = minth < 0 ? thickness / 2 : minth;
            return float.Max(nth, float.Sin(((i - nextdot) / mdots) * PI) * thickness);
        }
    }

    public static unsafe bool BeginPopupModal(ReadOnlySpan<char> name, ImGuiWindowFlags flags)
    {
        Span<byte> numPtr;
        if (name != null!)
        {
            var utf8ByteCount = Encoding.UTF8.GetByteCount(name);
            numPtr = utf8ByteCount <= 2048 ? stackalloc byte[utf8ByteCount + 1] : throw new ArgumentOutOfRangeException(nameof(name), "Can't have a name of more than 2048 UTF8 bytes");
            int utf8;
            fixed (byte* numPtr1 = numPtr)
                utf8 = Util.GetUtf8(name, numPtr1, utf8ByteCount);
            numPtr[utf8] = (byte)0;
        }
        else
            throw new ArgumentNullException(nameof(name));

        int num2;
        fixed (byte* numPtr1 = numPtr)
            num2 = (int)ImGuiNative.igBeginPopupModal(numPtr1, null, flags);
        return (uint)num2 > 0U;
    }

    public static unsafe bool BeginPopupModal(string name, ImGuiWindowFlags flags)
    {
        return BeginPopupModal(name.ToCharArray().AsSpan(), flags);
    }

    #region Input
    public static bool InputText(string label, string text, ref string changed)
    {
        if (!EditTexts.TryGetValue(label, out var edit))
        {
            edit = EditTexts[label] = new EditText(text);
        }
        edit.UpdateText(text);
        return edit.Draw(ref changed);
    }

    public static bool InputInt(string label, ref int value, ref int changed, bool displayHex = false)
    {
        if (!EditInts.TryGetValue(label, out var edit))
        {
            edit = EditInts[label] = new EditInt(value);
        }
        return edit.Draw(ref changed, displayHex);
    }

    private static readonly Dictionary<string, EditText> EditTexts = new();
    private static readonly Dictionary<string, EditInt> EditInts = new();

    private class EditText(string text)
    {
        public bool Editing;
        private string _text = text;
        private string _buffer = text;

        public void UpdateText(string text)
        {
            _text = text;
            _buffer = text;
        }

        public bool Draw(ref string buffer)
        {
            if (!Editing)
            {
                ImGui.TextUnformatted(_text);
                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    foreach (var (_, editText) in EditTexts)
                    {
                        editText.Editing = false;
                    }

                    foreach (var (_, editInt) in EditInts)
                    {
                        editInt.Editing = false;
                    }
                    Editing = true;
                }
            }
            else
            {
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                if (ImGui.InputText("##edit", ref _buffer, 100, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    Editing = false;
                }
                if (Editing)
                {
                    ImGui.SetKeyboardFocusHere(-1);
                }
                ImGui.PopStyleVar(2);
            }
            buffer = _buffer;

            var shouldTrue = _text != _buffer && !Editing;
            if (shouldTrue)
            {
                _text = _buffer;
            }
            return shouldTrue;
        }
    }

    private class EditInt(int inp)
    {
        public bool Editing;
        private int _value = inp;
        private int _buffer = inp;

        public bool Draw(ref int buffer, bool displayHex)
        {
            if (!Editing)
            {
                ImGui.TextUnformatted(_value.ToString());
                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    Editing = true;
                }
            }
            else
            {
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                if (ImGui.InputInt("##edit", ref _buffer, 1, 100, ImGuiInputTextFlags.EnterReturnsTrue | (displayHex ? ImGuiInputTextFlags.CharsHexadecimal : ImGuiInputTextFlags.CharsDecimal)))
                {
                    foreach (var (_, editText) in EditTexts)
                    {
                        editText.Editing = false;
                    }

                    foreach (var (_, editInt) in EditInts)
                    {
                        editInt.Editing = false;
                    }
                    Editing = false;
                }
                if (Editing)
                {
                    ImGui.SetKeyboardFocusHere(-1);
                    if (ImGui.IsKeyReleased(ImGuiKey.UpArrow))
                        _buffer += 1;
                    if (ImGui.IsKeyReleased(ImGuiKey.DownArrow))
                        _buffer -= 1;
                }
                ImGui.PopStyleVar(2);
            }
            buffer = _buffer;

            var shouldTrue = _value != _buffer && !Editing;
            if (shouldTrue)
            {
                _value = _buffer;
            }
            return shouldTrue;
        }
    }
    #endregion

    public static object GetStyleObject(ImGuiStyleVar imGuiStyleVar)
    {
        return (object?)GetStyleFloat(imGuiStyleVar) ?? GetStyleVector2(imGuiStyleVar) ?? throw new ArgumentOutOfRangeException(nameof(imGuiStyleVar), imGuiStyleVar, null);
    }

    public static float? GetStyleFloat(ImGuiStyleVar imGuiStyleVar)
    {
        var style = ImGui.GetStyle();
        return imGuiStyleVar switch
        {
            ImGuiStyleVar.Alpha => style.Alpha,
            ImGuiStyleVar.DisabledAlpha => style.DisabledAlpha,
            ImGuiStyleVar.WindowRounding => style.WindowRounding,
            ImGuiStyleVar.WindowBorderSize => style.WindowBorderSize,
            ImGuiStyleVar.WindowMinSize => style.GrabMinSize,
            ImGuiStyleVar.ChildBorderSize => style.ChildBorderSize,
            ImGuiStyleVar.ChildRounding => style.FrameRounding,
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
            ImGuiStyleVar.SeparatorTextBorderSize => style.SeparatorTextBorderSize,
            ImGuiStyleVar.DockingSeparatorSize => style.DockingSeparatorSize,
            _ => null
        };
    }

    public static Vector2? GetStyleVector2(ImGuiStyleVar imGuiStyleVar)
    {
        var style = ImGui.GetStyle();
        return imGuiStyleVar switch
        {
            ImGuiStyleVar.WindowPadding => style.WindowPadding,
            ImGuiStyleVar.WindowTitleAlign => style.WindowTitleAlign,
            ImGuiStyleVar.FramePadding => style.FramePadding,
            ImGuiStyleVar.ItemSpacing => style.ItemSpacing,
            ImGuiStyleVar.ItemInnerSpacing => style.ItemInnerSpacing,
            ImGuiStyleVar.CellPadding => style.CellPadding,
            ImGuiStyleVar.ButtonTextAlign => style.ButtonTextAlign,
            ImGuiStyleVar.SelectableTextAlign => style.SelectableTextAlign,
            ImGuiStyleVar.SeparatorTextAlign => style.SeparatorTextAlign,
            ImGuiStyleVar.SeparatorTextPadding => style.SeparatorTextPadding,
            _ => null
        };
    }
}

[Flags]
public enum ImGuiItemFlags
{
    None = 0,
    NoTabStop = 1 << 0,
    ButtonRepeat = 1 << 1,
    Disabled = 1 << 2,
    NoNav = 1 << 3,
    NoNavDefaultFocus = 1 << 4,
    SelectableDontClosePopup = 1 << 5,
    MixedValue = 1 << 6,
    ReadOnly = 1 << 7,
    NoWindowHoverableCheck = 1 << 8,
    AllowOverlap = 1 << 9,
    Inputable = 1 << 10
}

public struct ImRect
{
    public Vector2 Min;
    public Vector2 Max;

    public ImRect(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }

    public ImRect(Vector4 vec)
    {
        Min = new Vector2(vec.X, vec.Y);
        Max = new Vector2(vec.Z, vec.W);
    }

    public ImRect(float x1, float y1, float x2, float y2)
    {
        Min = new Vector2(x1, y1);
        Max = new Vector2(x2, y2);
    }

    public Vector2 Center => (Min + Max) / 2;
    public Vector2 Size => Max - Min;
    public float Width => Max.X - Min.X;
    public float Height => Max.Y - Min.Y;
    public float Area => Width * Height;
    public Vector2 TL => Min;
    public Vector2 TR => new Vector2(Max.X, Min.Y);
    public Vector2 BL => new Vector2(Min.X, Max.Y);
    public Vector2 BR => Max;
    bool Contains(Vector2 point) => point.X >= Min.X && point.X < Max.X && point.Y >= Min.Y && point.Y < Max.Y;
    bool Contains(ImRect rect) => rect.Min.X >= Min.X && rect.Min.Y >= Min.Y && rect.Max.X < Max.X && rect.Max.Y < Max.Y;
    bool ContainsWithPad(Vector2 point, Vector2 pad) => point.X >= Min.X - pad.X && point.X < Max.X + pad.X && point.Y >= Min.Y - pad.Y && point.Y < Max.Y + pad.Y;
    bool Overlaps(ImRect r) => r.Min.Y < Max.Y && r.Max.Y > Min.Y && r.Min.X < Max.X && r.Max.X > Min.X;
    void Add(Vector2 point)
    {
        Min = Vector2.Min(Min, point);
        Max = Vector2.Max(Max, point);
    }
    void Add(ImRect rect)
    {
        Min = Vector2.Min(Min, rect.Min);
        Max = Vector2.Max(Max, rect.Max);
    }

    void Expand(Vector2 amount)
    {
        Min -= amount;
        Max += amount;
    }

    void Expand(float amount)
    {
        Min -= new Vector2(amount, amount);
        Max += new Vector2(amount, amount);
    }

    void Translate(Vector2 d)
    {
        Min += d;
        Max += d;
    }

    void TranslateX(float dx)
    {
        Min.X += dx;
        Max.X += dx;
    }

    void TranslateY(float dy)
    {
        Min.Y += dy;
        Max.Y += dy;
    }

    void ClipWith(ImRect clip)
    {
        Min = Vector2.Max(Min, clip.Min);
        Max = Vector2.Min(Max, clip.Max);
    }

    void ClipWithFull(ImRect clip)
    {
        Min = Vector2.Clamp(Min, clip.Min, clip.Max);
        Max = Vector2.Clamp(Max, clip.Min, clip.Max);
    }

    void Floor()
    {
        Min = new Vector2(float.Floor(Min.X), float.Floor(Min.Y));
        Max = new Vector2(float.Floor(Max.X), float.Floor(Max.Y));
    }

    bool IsInverted() => Min.X > Max.X || Min.Y > Max.Y;

    Vector4 ToVec4() => new(Min.X, Min.Y, Max.X, Max.Y);
}