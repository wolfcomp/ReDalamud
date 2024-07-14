using System.Globalization;

namespace ReDalamud.Standalone.Types;
public class ClassRenderer : IRenderer, IComparable<ClassRenderer>
{
    public bool HasCode => true;
    public int Size => Renderers.OrderBy(t => t.Key).Last().Key; // start with 64 bytes for now this will be able to be changed later *need to actually code it*
    public Dictionary<int, IRenderer> Renderers = new();
    public bool IsCollapsed = false;
    public nint Address = (nint)0x140000000;
    public string Name = GenerateRandomName();
    public string OffsetText = "140000000";
    private string SizeString => Config.Global.DisplayAsHex ? "0x" + Size.ToString("X") : Size.ToString();
    private float _height = -1;

    public ClassRenderer()
    {
        Address = MemoryRead.OpenedProcess.MainModule!.BaseAddress;
        OffsetText = Address.ToString("X");
        // Maybe make the default size configurable?
        for (var i = 0; i < 0x40; i += 8)
        {
            Renderers.Add(i, new Unknown8Renderer());
        }
    }

    public ClassRenderer(nint address) : this()
    {
        Address = address;
        OffsetText = address.ToString("X");
    }

    private static string GenerateRandomName()
    {
        return $"N{Rand.Next(0, 0xFFFFFF):X8}";
    }

    public bool DrawName(bool isSelected = false)
    {
        // TODO: Add check if the class has an instance and change the color of the name to address color
        //ImGuiExt.PushTextStyleColor(Config.Styles.AddressColor);
        var selected = ImGui.Selectable(Name, isSelected, ImGuiSelectableFlags.SpanAllColumns);
        //ImGui.PopStyleColor();
        return selected;
    }

    public void DrawMemory(nint address)
    {
        if (address == nint.Zero)
            address = Address;
        DrawHeader(address);
        if (IsCollapsed)
            return;

        var minWindow = ImGui.GetWindowContentRegionMin();
        var maxWindow = ImGui.GetWindowContentRegionMax();
        var scrollY = ImGui.GetScrollY();
        var windowHeight = maxWindow.Y - minWindow.Y;
        ImGui.BeginChild($"ClassRendererMemory##{Name}{address}", new Vector2(-1, GetHeight()));
        ImGui.Indent();
        var posY = 0f;
        foreach (var (offset, renderer) in Renderers)
        {
            //if (!isVisible(posY))
            //{
            //    posY += renderer.GetHeight();
            //    continue;
            //}
            ImGuiExt.PushTextStyleColor(Config.Styles.OffsetColor);
            ImGui.Text($"{offset:X4}");
            ImGui.PopStyleColor();
            ImGui.SameLine();
            renderer.DrawMemory(address + offset);
            posY += renderer.GetHeight();
        }
        ImGui.Unindent();
        ImGui.EndChild();
        return;

        bool isVisible(float y)
        {
            return y >= scrollY && y <= windowHeight + scrollY;
        }
    }

    private void DrawHeader(nint address)
    {
        ImGui.BeginChild($"ClassHeader##{Name}{address}", new Vector2(-1, ImGui.GetTextLineHeight()));
        var style = ImGuiSmrt.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        style.Push(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        style.Push(ImGuiStyleVar.ItemInnerSpacing, new Vector2(0, 0));
        var color = ImGuiSmrt.PushColor(ImGuiCol.Button, Config.Styles.BackgroundColor);
        if (ImGui.ImageButton($"Collapse##{Name}{address}", IconLoader.GetIconTextureId(IsCollapsed ? Icon16.ClosedIcon : Icon16.OpenIcon), new Vector2(16)))
        {
            IsCollapsed = !IsCollapsed;
        }
        ImGui.SameLine();
        ImGui.Image(IconLoader.GetIconTextureId(Icon16.ClassType), new Vector2(16));
        style.Dispose();
        ImGui.SameLine();
        color.PushTextColor(Config.Styles.AddressColor);
        var changed = "";
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        if (ImGuiExt.InputText($"AddressEdit##{address}", OffsetText, ref changed))
        {
            OffsetText = changed.Trim();
            if (OffsetText[0] is '<' or '[')
            {
                // TODO: Get the address from data.yml in directory of Config.Global.ClientStructsPath
            }
            else if (nint.TryParse(OffsetText, NumberStyles.AllowHexSpecifier, null, out nint parsedAddress))
                Address = parsedAddress;
            OffsetText = Address.ToString("X");
        }
        ImGui.SameLine();
        color.PushTextColor(Config.Styles.TypeColor);
        ImGui.Text("Class");
        ImGui.SameLine();
        color.PushTextColor(Config.Styles.NameColor);
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        if (ImGuiExt.InputText($"NameEdit##{address}", Name, ref changed))
        {
            Name = changed.Trim();
        }
        ImGui.SameLine();
        color.PushTextColor(Config.Styles.ValueColor);
        ImGui.TextUnformatted($"[{SizeString}]");
        color.Dispose();
        ImGui.EndChild();
        if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            ImGui.OpenPopup($"ClassPopup##{Name}{address}");
        DrawPopUp(address);
    }

    public void DrawPopUp(nint address)
    {
        if (ImGui.BeginPopupContextWindow($"ClassPopup##{Name}{address}"))
        {
            if (ImGui.Selectable("Copy Address"))
            {
                ImGui.SetClipboardText(Address.ToString("X"));
            }

            if (ImGuiExt.MenuWithIcon(Icon16.ButtonAdd, "Add Bytes", $"ClassAddBytes##{Name}{address}"))
            {
                var last = Renderers.OrderBy(t => t.Key).Last().Key + 8;
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes8, "Add 8 Bytes"))
                {
                    Renderers.Add(last, new Unknown8Renderer());
                    _height = -1;
                }

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes64, "Add 64 Bytes"))
                {
                    _height = -1;
                    for (var i = 0; i < 64; i += 8)
                    {
                        Renderers.Add(last + i, new Unknown8Renderer());
                    }
                }

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes256, "Add 256 Bytes"))
                {
                    _height = -1;
                    for (var i = 0; i < 256; i += 8)
                    {
                        Renderers.Add(last + i, new Unknown8Renderer());
                    }
                }

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes1024, "Add 1024 Bytes"))
                {
                    _height = -1;
                    for (var i = 0; i < 1024; i += 8)
                    {
                        Renderers.Add(last + i, new Unknown8Renderer());
                    }
                }

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes4096, "Add 4096 Bytes"))
                {
                    _height = -1;
                    for (var i = 0; i < 4096; i += 8)
                    {
                        Renderers.Add(last + i, new Unknown8Renderer());
                    }
                }

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytesX, "Add Enough Bytes"))
                {
                    _height = -1;
                    for (var i = 0; i < 0x800000; i += 8)
                    {
                        Renderers.Add(last + i, new Unknown8Renderer());
                    }
                }

                ImGui.EndMenu();
            }
            ImGui.EndPopup();
        }
    }

    public float GetHeight()
    {
        if (_height > 0)
            return _height;
        var height = ImGui.GetTextLineHeightWithSpacing();
        return _height = height + Renderers.Sum(t => t.Value.GetHeight());
    }

    public void DrawCSharpCode()
    {

    }

    public int CompareTo(ClassRenderer? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        var isCollapsedComparison = IsCollapsed.CompareTo(other.IsCollapsed);
        if (isCollapsedComparison != 0)
        {
            return isCollapsedComparison;
        }

        var addressComparison = Address.CompareTo(other.Address);
        if (addressComparison != 0)
        {
            return addressComparison;
        }

        var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
        if (nameComparison != 0)
        {
            return nameComparison;
        }

        return string.Compare(OffsetText, other.OffsetText, StringComparison.Ordinal);
    }
}