using Windows.Foundation.Collections;

namespace ReDalamud.Standalone.Types;
public class Unknown2Renderer : UnknownBaseRenderer
{
    public override bool HasName => false;
    public override bool HasCode => false;
    public override int Size => 2;
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
        var valueInt = BitConverter.ToInt16(_bytes);
        var valueUInt = BitConverter.ToUInt16(_bytes);
        using var _ = ImGuiSmrt.PushDefaultColor();
        ImGui.BeginTooltip();
        ImGui.TextUnformatted($"Int: {valueInt}");
        ImGui.TextUnformatted($"UInt: 0x{valueUInt:X4}");
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