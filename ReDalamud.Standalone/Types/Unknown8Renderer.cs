namespace ReDalamud.Standalone.Types;
internal class Unknown8Renderer : IRenderer
{
    public bool HasCode => false;
    public int Size => 8;
    public void DrawMemory(nint address)
    {
        var bytes = MemoryRead.ReadBytes(address, Size);
        var valueInt = BitConverter.ToInt64(bytes);
        var valueFloat = BitConverter.ToDouble(bytes).ToString("F3");
        var valueHex = string.Join("", BitConverter.ToString(bytes).Split('-').Reverse()).TrimStart('0');
        if(!string.IsNullOrWhiteSpace(valueHex))
        {
            valueHex = "0x" + valueHex;
        }
        ImGuiExt.PushTextStyleColor(Config.Styles.AddressColor);
        ImGui.TextUnformatted(address.ToString("X16"));
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.TextColor);
        ImGui.TextUnformatted(MemoryRead.CharFromBytes(bytes));
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.HexValueColor);
        ImGui.TextUnformatted(string.Join(' ', bytes.Select(t => t.ToString("X2"))));
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.CommentColor);
        ImGui.TextUnformatted("//");
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.ValueColor);
        ImGui.TextUnformatted($"{valueFloat} {valueInt} {valueHex}");
        ImGui.PopStyleColor();
        var pointsTo = MemoryRead.IsInRegion((nint)valueInt);
        if (string.IsNullOrWhiteSpace(pointsTo))
            return;
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.OffsetColor);
        ImGui.TextUnformatted($"-> <{pointsTo.ToUpper()}>{valueHex[2..]}");
        ImGui.PopStyleColor();
    }

    public void DrawCSharpCode()
    {

    }
}
