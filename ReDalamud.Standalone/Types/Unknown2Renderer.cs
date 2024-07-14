using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace ReDalamud.Standalone.Types;
public class Unknown2Renderer : IUnknownRenderer
{
    public bool HasCode => false;
    public int Size => 2;
    private float _height = -1;
    private byte[] _bytes = [];
    public void DrawMemory(nint address, int offset)
    {
        var bytes = MemoryRead.ReadBytes(address, Size);
        _bytes = bytes;
        using var style = ImGuiSmrt.PushTextColor(Config.Styles.OffsetColor);
        ImGui.Text($"{offset:X4}");
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.AddressColor);
        ImGui.TextUnformatted(address.ToString("X8"));
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.TextColor);
        ImGui.TextUnformatted(MemoryRead.CharFromBytes(bytes));
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.HexValueColor);
        ImGui.TextUnformatted(string.Join(' ', bytes.Select(t => t.ToString("X2"))));
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.CommentColor);
        ImGui.TextUnformatted("//");
    }

    public void DrawToolTip()
    {
        var valueInt = BitConverter.ToInt16(_bytes);
        var valueUInt = BitConverter.ToUInt16(_bytes);
        using var _ = ImGuiSmrt.PushDefaultColor();
        ImGui.BeginTooltip();
        ImGui.TextUnformatted($"Int: {valueInt}");
        ImGui.TextUnformatted($"UInt: 0x{valueUInt:X4}");
        ImGui.EndTooltip();
    }

    public void DrawCSharpCode()
    {

    }

    public float GetHeight()
    {
        if (_height > 0)
            return _height;
        return _height = ImGui.GetTextLineHeight() + ImGui.GetStyle().ItemSpacing.Y;
    }
}