namespace ReDalamud.Standalone.Types;

public class Unknown8Renderer : UnknownBaseRenderer
{
    public override bool HasName => true;
    public override bool HasCode => false;
    public override int Size => 8;
    public override string FieldName { get; set; } = "";
    private float _height = -1;
    private byte[] _bytes = [];

    public override void DrawMemory(nint address, int offset)
    {
        if (string.IsNullOrWhiteSpace(FieldName))
            FieldName = $"field_{offset:X}";
        var bytes = MemoryRead.ReadBytes(address, Size);
        _bytes = bytes;
        var valueInt64 = BitConverter.ToInt64(bytes);
        var valueFloat = BitConverter.ToSingle(bytes);
        var stringFloat = valueFloat is > -999999.0f and < 999999.0f ? float.Round(valueFloat, 3).ToString("F3") : "#####";
        var valueHex = string.Join("", BitConverter.ToString(bytes).Split('-').Reverse()).TrimStart('0');
        if (!string.IsNullOrWhiteSpace(valueHex))
        {
            valueHex = "0x" + valueHex;
        }

        DrawLine(offset, address, bytes, $"{stringFloat} {valueInt64} {valueHex}");

        string? pointsTo;
        var vtable = ClientStructsData?.VtableLookup.FindFromNeedle(t => t.Vtable.Ea, (a, b) => (a.Vtable.Ea == (ulong)valueInt64 || b.Vtable.Ea == (ulong)valueInt64, a.Vtable.Ea == (ulong)valueInt64), (ulong)valueInt64);
        if (vtable != null && vtable.Vtable.Ea == (ulong)valueInt64)
        {
            pointsTo = vtable.ClassName;
            if (vtable.Index != 0)
                pointsTo += $"_{(string.IsNullOrWhiteSpace(vtable.Vtable.Base) ? vtable.Index : vtable.Vtable.Base)}";
        }
        else
            pointsTo = MemoryRead.IsInRegion((nint)valueInt64);
        if (string.IsNullOrWhiteSpace(pointsTo))
            return;
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, Config.Styles.OffsetColor.InternalValue);
        ImGui.TextUnformatted($"-> <{pointsTo.ToUpper()}>{valueHex[2..]}");
    }

    public override void DrawCSharpCode()
    {

    }

    public override float GetHeight()
    {
        if (_height > 0)
            return _height;
        return _height = ImGui.GetTextLineHeight() + ImGui.GetStyle().ItemSpacing.Y;
    }

    public override void DrawToolTip()
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
