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
    /*
     * Required for Docking
     * DockBuilderDockWindow
     * DockBuilderGetNode
     * DockBuilderGetCentralNode
     * DockBuilderAddNode
     * DockBuilderRemoveNode
     * DockBuilderRemoveNodeDockedWindows
     * DockBuilderRemoveNodeChildNodes
     * DockBuilderSetNodePos
     * DockBuilderSetNodeSize
     * DockBuilderSplitNode
     * DockBuilderCopyDockSpace
     * DockBuilderCopyNode
     * DockBuilderCopyWindowSettings
     * DockBuilderFinish
     */

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

    public static void PushTextStyleColor(Color color)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, (Vector4)color);
    }

    public static bool InputText(string label, ref string text, ref string changed)
    {
        if (!EditTexts.TryGetValue(label, out var edit))
        {
            edit = EditTexts[label] = new EditText(text);
        }
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
                    if(ImGui.IsKeyReleased(ImGuiKey.UpArrow))
                        _buffer += 1;
                    if(ImGui.IsKeyReleased(ImGuiKey.DownArrow))
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
}
