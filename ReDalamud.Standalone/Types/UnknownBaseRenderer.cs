using NoAlloq;

namespace ReDalamud.Standalone.Types;

public abstract class UnknownBaseRenderer : IUnknownRenderer
{
    public abstract bool HasName { get; }
    public abstract bool HasCode { get; }
    public abstract int Size { get; }
    public abstract string FieldName { get; set; }
    public abstract void DrawMemory(nint address, int offset);

    public abstract void DrawCSharpCode();

    public abstract float GetHeight();

    public abstract void DrawToolTip();

    public void DrawLine(int offset, nint address, Span<byte> bytes, string? valueText = null)
    {

        using var style = ImGuiSmrt.PushTextColor(Config.Styles.OffsetColor);
        ImGui.Text($"{offset:X4}");
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.AddressColor);
        ImGui.TextUnformatted(address.ToString("X16"));
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.TextColor);
        ImGui.TextUnformatted(MemoryRead.CharFromBytes(bytes));
        ImGui.SameLine();
        if (Config.Global.ShowNameOnUnknown)
        {
            style.PushTextColor(Config.Styles.NameColor);
            ImGui.TextUnformatted(FieldName);
            ImGui.SameLine();
        }
        style.PushTextColor(Config.Styles.HexValueColor);
        ImGui.TextUnformatted(string.Join(' ', bytes.Select(t => t.ToString("X2")).ToArray()));
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.CommentColor);
        ImGui.TextUnformatted("//");
        if (valueText == null) return;
        ImGui.SameLine();
        style.PushTextColor(Config.Styles.ValueColor);
        ImGui.TextUnformatted(valueText);
    }
}