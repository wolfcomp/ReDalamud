namespace ReDalamud.Standalone.Types;
public class Unknown4Renderer : UnknownBaseRenderer
{
    public override bool HasName => false;
    public override bool HasCode => false;
    public override int Size => 4;
    public override string FieldName { get; set; } = "";
    private float _height = -1;
    private byte[] _bytes = [];
    public override void DrawMemory(nint address, int offset)
    {
        var bytes = MemoryRead.ReadBytes(address, Size);
        _bytes = bytes;
        var valueInt = BitConverter.ToInt32(bytes);
        var valueFloat = BitConverter.ToSingle(bytes);
        var stringFloat = valueFloat is > -999999.0f and < 999999.0f ? float.Round(valueFloat, 3).ToString("F3") : "#####";
        var valueHex = string.Join("", BitConverter.ToString(bytes).Split('-').Reverse()).TrimStart('0');
        if (!string.IsNullOrWhiteSpace(valueHex))
        {
            valueHex = "0x" + valueHex;
        }
        
        DrawLine(offset, address, bytes, $"{stringFloat} {valueInt} {valueHex}");
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
