namespace ReDalamud.Standalone.Types;
public class Unknown1Renderer : UnknownBaseRenderer
{
    public override bool HasName => false;
    public override bool HasCode => false;
    public override int Size => 1;
    public override string FieldName { get; set; } = "";
    private float _height = -1;
    private byte[] _bytes = [];
    public override void DrawMemory(nint address, int offset)
    {
        var bytes = MemoryRead.ReadBytes(address, Size);
        _bytes = bytes;
        
        DrawLine(offset, address, bytes);
    }

    public override void DrawToolTip()
    {
        var valueInt = Convert.ToSByte(_bytes[0]);
        var valueUInt = _bytes[0];
        using var _ = ImGuiSmrt.PushDefaultColor();
        ImGui.BeginTooltip();
        ImGui.TextUnformatted($"Int: {valueInt}");
        ImGui.TextUnformatted($"UInt: 0x{valueUInt:X2}");
        ImGui.EndTooltip();
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
}
