namespace ReDalamud.Standalone.Types;
public class Unknown8Renderer : IUnknownRenderer
{
    public bool HasName => false;
    public bool HasCode => false;
    public int Size => 8;
    private float _height = -1;
    private byte[] _bytes = [];
    public void DrawMemory(nint address, int offset)
    {
        var bytes = MemoryRead.ReadBytes(address, Size);
        _bytes = bytes;
        var valueInt64 = BitConverter.ToInt64(bytes);
        var valueFloat = BitConverter.ToSingle(bytes);
        var stringFloat = valueFloat is > -999999.0f and < 999999.0f ? float.Round(valueFloat, 3).ToString("F3") : "#####";
        var valueHex = string.Join("", BitConverter.ToString(bytes).Split('-').Reverse()).TrimStart('0');
        if(!string.IsNullOrWhiteSpace(valueHex))
        {
            valueHex = "0x" + valueHex;
        }

        using var style = ImGuiSmrt.PushTextColor(Config.Styles.OffsetColor);
        ImGui.Text($"{offset:X4}");
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.AddressColor);
        ImGui.TextUnformatted(address.ToString("X16"));
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
        ImGui.TextUnformatted($"{stringFloat} {valueInt64} {valueHex}");
        var pointsTo = MemoryRead.IsInRegion((nint)valueInt64);
        if (string.IsNullOrWhiteSpace(pointsTo))
            return;
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.OffsetColor);
        ImGui.TextUnformatted($"-> <{pointsTo.ToUpper()}>{valueHex[2..]}");
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
        var valueInt64 = BitConverter.ToInt64(_bytes);
        var valueUInt64 = BitConverter.ToUInt64(_bytes);
        var valueFloat = BitConverter.ToSingle(_bytes);
        var valueDouble = BitConverter.ToDouble(_bytes);
        ImGui.BeginTooltip();
        ImGui.TextUnformatted($"Int64: {valueInt64}");
        ImGui.TextUnformatted($"UInt64: 0x{valueUInt64:X16}");
        ImGui.TextUnformatted($"Float: {valueFloat}");
        ImGui.TextUnformatted($"Double: {valueDouble}");
        ImGui.EndTooltip();
    }
}
