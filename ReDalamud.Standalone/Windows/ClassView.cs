using ReDalamud.Standalone.Utils;
using System.Globalization;
using System.Text;

namespace ReDalamud.Standalone.Windows;
public class ClassView(string offset) : ICloneable
{
    public string Offset = offset;
    public string Name = GenerateRandomName();
    public int Size = 0x80;
    private bool _displaySizeAsHex;
    private bool _isCollapsed;
    private nint _offset = nint.Parse(offset, NumberStyles.AllowHexSpecifier);

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

        var bytes = MemoryRead.ReadBytes(_offset, Size);

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
                        if (i + j >= bytes.Length) break;
                        ImGui.Text($"{GetByteChar(bytes[i + j])}");
                        ImGui.SameLine(0, 0);
                    }
                    ImGui.SameLine();
                    ImGui.PopStyleColor();
                    ImGuiExt.PushTextStyleColor(Config.Styles.HexValueColor);
                    for (var j = 0; j < 8; j++)
                    {
                        if (i + j >= bytes.Length) break;
                        ImGui.Text($"{bytes[i + j]:X2}");
                        ImGui.SameLine();
                    }
                    ImGui.PopStyleColor();
                    ImGuiExt.PushTextStyleColor(Config.Styles.ValueColor);
                    if (i + 8 <= bytes.Length)
                    {
                        var dob = ConvertBits.ToDouble(bytes, i).ToString("F3");
                        ImGui.Text($"{dob}");
                        ImGui.SameLine();
                        var lng = ConvertBits.ToInt64(bytes, i);
                        ImGui.Text($"{lng}");
                    }
                    else
                    {
                        if (i + 4 == bytes.Length)
                        {
                            var flt = ConvertBits.ToSingle(bytes, i).ToString("F3");
                            ImGui.Text($"{flt}");
                            ImGui.SameLine();
                            var lng = ConvertBits.ToInt32(bytes, i);
                            ImGui.Text($"{lng}");
                        }
                        else if (i + 2 == bytes.Length)
                        {
                            var sht = ConvertBits.ToInt16(bytes, i);
                            ImGui.Text($"{sht}");
                            ImGui.SameLine();
                            var usht = ConvertBits.ToUInt16(bytes, i);
                            ImGui.Text($"{usht}");
                        }
                        else
                        {
                            var b = bytes[i];
                            ImGui.Text($"{b}");
                        }
                    }
                    var hex = ConvertBits.ToString(bytes, i, Math.Min(8, bytes.Length - i - 1)).Replace("-", "").TrimStart('0');
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
        var changed = "";
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        if (ImGuiExt.InputText($"OffsetEdit##{Offset}", ref Offset, ref changed))
        {
            Offset = changed.Trim();
            _offset = nint.Parse(Offset, NumberStyles.AllowHexSpecifier);
        }
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.TypeColor);
        ImGui.Text("Class");
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.NameColor);
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        if (ImGuiExt.InputText($"NameEdit##{Offset}", ref Name, ref changed))
        {
            Name = changed.Trim();
        }
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.ValueColor);
        var sizeChanged = 0;
        if (ImGuiExt.InputInt($"SizeEdit##{Offset}", ref Size, ref sizeChanged, _displaySizeAsHex))
        {
            Size = sizeChanged;
        }
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
        return (_offset + offset).ToString("X").PadLeft(nint.Size * 2, '0');
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
