using ReDalamud.Standalone.Utils;
using System.Text;

namespace ReDalamud.Standalone.Windows;
public class ClassView(nint offset) : ICloneable
{
    public nint Offset = offset;
    public string Name = GenerateRandomName();
    public int Size = 0x80;
    private bool _displaySizeAsHex = false;
    private bool _isCollapsed = false;

    private static string GenerateRandomName()
    {
        return $"N{Rand.Next(0, 0xFFFFFF):X8}";
    }

    public void Draw(bool isStatic = false, bool isInherited = false)
    {
        if (!isStatic)
        {
            ImGui.PushStyleColor(ImGuiCol.WindowBg, (Vector4)Config.Styles.BackgroundColor);
            ImGui.Begin($"ClassView##{Offset}");
        }

        var bytes = MemoryRead.ReadBytes(Offset, Size);

        if (bytes.Length > 0)
        {
            DrawHeader();
            if (!_isCollapsed)
            {
                ImGui.Indent();
                for (var i = 0; i < bytes.Length; i += 8)
                {
                    ImGuiExt.PushTextStyleColor(Config.Styles.OffsetColor);
                    ImGui.Text($"{GetOffsetString(i, Size - 1)}");
                    ImGui.PopStyleColor();
                    ImGui.SameLine();
                    ImGuiExt.PushTextStyleColor(Config.Styles.AddressColor);
                    ImGui.Text($"{GetMemoryOffsetString(i)}");
                    ImGui.PopStyleColor();
                    ImGui.SameLine();
                    ImGuiExt.PushTextStyleColor(Config.Styles.TextColor);
                    for (var j = 0; j < 8; j++)
                    {
                        ImGui.Text($"{GetByteChar(bytes[i + j])}");
                        ImGui.SameLine(0, 0);
                    }
                    ImGui.SameLine();
                    ImGui.PopStyleColor();
                    ImGuiExt.PushTextStyleColor(Config.Styles.HexValueColor);
                    for (var j = 0; j < 8; j++)
                    {
                        ImGui.Text($"{bytes[i + j]:X2}");
                        ImGui.SameLine();
                    }
                    ImGui.PopStyleColor();
                    ImGuiExt.PushTextStyleColor(Config.Styles.ValueColor);
                    var dob = double.Round(ConvertBits.ToDouble(bytes, i), 3, MidpointRounding.ToEven);
                    ImGui.Text($"{dob}");
                    ImGui.SameLine();
                    var lng = ConvertBits.ToInt64(bytes, i);
                    ImGui.Text($"{lng}");
                    var hex = ConvertBits.ToString(bytes, i, 8).Replace("-", "").TrimStart('0');
                    if (!string.IsNullOrWhiteSpace(hex))
                    {
                        ImGui.SameLine();
                        ImGui.Text($"0x{hex}");
                    }
                    ImGui.PopStyleColor();
                }
                ImGui.Unindent();
            }
        }

        if (!isStatic)
        {
            ImGui.PopStyleColor();
            ImGui.End();
        }
    }

    private unsafe void DrawHeader()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        ImGui.PushStyleColor(ImGuiCol.Button, (Vector4)Config.Styles.BackgroundColor);
        if (ImGui.ImageButton($"ClassViewCollapse##{Name}{Offset}", IconLoader.GetIconTextureId(_isCollapsed ? Icon16.ClosedIcon : Icon16.OpenIcon), new Vector2(16)))
        {
            _isCollapsed = !_isCollapsed;
        }
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGui.Image(IconLoader.GetIconTextureId(Icon16.ClassType), new Vector2(16));
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.AddressColor);
        ImGui.Text($"{Offset:X}");
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.TypeColor);
        ImGui.Text("Class");
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.NameColor);
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        if (ImGui.InputText($"##{Offset}", ref Name, 2048))
        {
            Name = Name.Trim();
        }
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.ValueColor);
        ImGui.Text($"[{(_displaySizeAsHex ? $"0x{Size:X}" : Size)}]");
        ImGui.PopStyleColor();
        if (ImGui.IsItemHovered() && ImGui.IsMouseReleased(ImGuiMouseButton.Right))
        {
            ImGui.OpenPopup($"ClassViewContextMenu##{Name}{Offset}");
        }
        DrawPopup();
    }

    private void DrawPopup()
    {
        if (ImGui.BeginPopup($"ClassViewContextMenu##{Name}{Offset}"))
        {
            if (ImGui.MenuItem("Hex", "", _displaySizeAsHex))
            {
                _displaySizeAsHex = !_displaySizeAsHex;
            }
            ImGui.EndPopup();
        }
    }

    private char GetByteChar(byte b) => b < 20 ? '.' : (char)b;

    private string GetMemoryOffsetString(nint offset)
    {
        return (Offset + offset).ToString("X").PadLeft(nint.Size * 2, '0');
    }

    private string GetOffsetString(int offset, int maxOffset)
    {
        return offset.ToString("X").PadLeft(int.Max(maxOffset.ToString("X").Length, 4), '0');
    }

    /// <summary>
    /// Updates size with the given addition.
    /// </summary>
    /// <param name="add">How much to increase or decrease size by.</param>
    public void UpdateSize(int add)
    {
        Size += add;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public object Clone()
    {
        return new ClassView(Offset) { Name = Name };
    }
}
