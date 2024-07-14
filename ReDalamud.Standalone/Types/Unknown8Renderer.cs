namespace ReDalamud.Standalone.Types;
internal class Unknown8Renderer : IRenderer
{
    public bool HasCode => false;
    public int Size => 8;
    private float _height = -1;
    public void DrawMemory(nint address)
    {
        var bytes = MemoryRead.ReadBytes(address, Size);
        var valueInt64 = BitConverter.ToInt64(bytes);
        var valueUInt64 = BitConverter.ToUInt64(bytes);
        var valueFloat = BitConverter.ToSingle(bytes);
        var valueDouble = BitConverter.ToDouble(bytes);
        var stringFloat = valueFloat is > -999999.0f and < 999999.0f ? float.Round(valueFloat, 3).ToString("F3") : "#####";
        var valueHex = string.Join("", BitConverter.ToString(bytes).Split('-').Reverse()).TrimStart('0');
        if(!string.IsNullOrWhiteSpace(valueHex))
        {
            valueHex = "0x" + valueHex;
        }

        using var style = ImGuiSmrt.PushTextColor(Config.Styles.AddressColor);
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
}
