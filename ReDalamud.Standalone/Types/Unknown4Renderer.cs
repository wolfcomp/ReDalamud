using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace ReDalamud.Standalone.Types;
public class Unknown4Renderer : IUnknownRenderer
{
    public bool HasCode => false;
    public int Size => 4;
    private float _height = -1;
    private byte[] _bytes = [];
    public void DrawMemory(nint address, int offset)
    {
        var bytes = MemoryRead.ReadBytes(address, Size);
        _bytes = bytes;
        var valueInt = BitConverter.ToInt32(bytes);
        var valueUInt = BitConverter.ToUInt32(bytes);
        var valueFloat = BitConverter.ToSingle(bytes);
        var stringFloat = valueFloat is > -999999.0f and < 999999.0f ? float.Round(valueFloat, 3).ToString("F3") : "#####";
        var valueHex = string.Join("", BitConverter.ToString(bytes).Split('-').Reverse()).TrimStart('0');
        if (!string.IsNullOrWhiteSpace(valueHex))
        {
            valueHex = "0x" + valueHex;
        }

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
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.ValueColor);
        ImGui.TextUnformatted($"{stringFloat} {valueInt} {valueHex}");
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

    public void DrawToolTip()
    {
        using var _ = ImGuiSmrt.PushDefaultColor();
        var valueInt = BitConverter.ToInt32(_bytes);
        var valueUInt = BitConverter.ToUInt32(_bytes);
        var valueFloat = BitConverter.ToSingle(_bytes);
        ImGui.BeginTooltip();
        ImGui.TextUnformatted($"Int: {valueInt}");
        ImGui.TextUnformatted($"UInt: 0x{valueUInt:X8}");
        ImGui.TextUnformatted($"Float: {valueFloat}");
        ImGui.EndTooltip();
    }
}
