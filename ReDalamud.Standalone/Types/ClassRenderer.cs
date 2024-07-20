using System.Globalization;

namespace ReDalamud.Standalone.Types;
public class ClassRenderer : IRenderer, IComparable<ClassRenderer>
{
    public bool HasCode => true;
    public int Size => Renderers.Sum(t => t.Size);
    public List<IRenderer> Renderers = new();
    public bool IsCollapsed;
    public nint Address = (nint)0x140000000;
    public string Name = GenerateRandomName();
    public string OffsetText = "140000000";
    private string SizeString => Config.Global.DisplayAsHex ? "0x" + Size.ToString("X") : Size.ToString();
    private float _height = -1;
    private bool _addingCustomBytes;
    private int _addingCustomBytesSize;
    private int _selectedIndex = -1;

    public ClassRenderer()
    {
        Address = MemoryRead.OpenedProcess.MainModule!.BaseAddress;
        OffsetText = Address.ToString("X");
        // Maybe make the default size configurable?
        for (var i = 0; i < 0x40; i += 8)
        {
            Renderers.Add(new Unknown8Renderer());
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

    public void DrawMemory(nint address, int offset)
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
        var posY = 0f;
        offset = 0;
        var childSize = new Vector2(-1, ImGui.GetTextLineHeight());
        for (var index = 0; index < Renderers.Count; index++)
        {
            //if (!isVisible(posY))
            //{
            //    posY += renderer.GetHeight();
            //    continue;
            //}
            var renderer = Renderers[index];
            if (index == _selectedIndex)
                ImGui.PushStyleColor(ImGuiCol.ChildBg, (Vector4)Config.Styles.SelectedColor);
            ImGui.BeginChild($"ClassRendererRow###{Name}{address}{index}", childSize, false, ImGuiWindowFlags.NoScrollbar);
            renderer.DrawMemory(address + offset, offset);
            ImGui.EndChild();
            var pos = ImGui.GetCursorPos();
            if (index == _selectedIndex)
                ImGui.PopStyleColor();
            if (ImGui.IsItemHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) _selectedIndex = index;
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    _selectedIndex = index;
                    ImGui.OpenPopup($"ClassPopup##{Name}{address}");
                }
            }

            posY += renderer.GetHeight();
            offset += renderer.Size;
        }
        DrawPopUp(address);
        ImGui.EndChild();
        return;

        bool isVisible(float y)
        {
            return y >= scrollY && y <= windowHeight + scrollY;
        }
    }

    private void DrawHeader(nint address)
    {
        if (_selectedIndex == -1)
            ImGui.PushStyleColor(ImGuiCol.ChildBg, (Vector4)Config.Styles.SelectedColor);
        ImGui.BeginChild($"ClassHeader##{Name}{address}", new Vector2(-1, ImGui.GetTextLineHeight()));
        var style = ImGuiSmrt.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        style.Push(ImGuiStyleVar.FrameRounding, 0);
        var color = ImGuiSmrt.PushColor(ImGuiCol.Button, Config.Styles.BackgroundColor);
        if (ImGui.ImageButton($"Collapse##{Name}{address}", IconLoader.GetIconTextureId(IsCollapsed ? Icon16.ClosedIcon : Icon16.OpenIcon), new Vector2(8)))
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
        if (ImGui.IsItemHovered())
        {
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) _selectedIndex = -1;
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                _selectedIndex = -1;
                ImGui.OpenPopup($"ClassPopup##{Name}{address}");
            }
        }

        if (_selectedIndex == -1)
            ImGui.PopStyleColor();
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

            ImGui.BeginDisabled(_selectedIndex == -1);
            if (ImGuiExt.MenuWithIcon(Icon16.ExchangeButton, "Change Type", $"ChangeType##{Name}{address}"))
            {
                // TODO add types and replace the current renderer with the selected type and pad with unknown types if needed
            }
            ImGui.EndDisabled();

            if (ImGuiExt.MenuWithIcon(Icon16.ButtonAdd, "Add Bytes", $"ClassAddBytes##{Name}{address}"))
            {
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes4, "Add 4 Byte"))
                    AddBytes(4);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes8, "Add 8 Bytes"))
                    AddBytes(8);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes64, "Add 64 Bytes"))
                    AddBytes(64);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes256, "Add 256 Bytes"))
                    AddBytes(256);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes1024, "Add 1024 Bytes"))
                    AddBytes(1024);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytes4096, "Add 4096 Bytes"))
                    AddBytes(4096);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytesX, "Add ... Bytes"))
                    _addingCustomBytes = true;

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonAddBytesX, "Add Enough Bytes"))
                    AddBytes(0x800000);

                ImGui.EndMenu();
            }
            ImGui.EndPopup();
        }

        if (_addingCustomBytes)
        {
            ImGui.OpenPopup($"Add Bytes##{Name}{address}");
            _addingCustomBytes = false;
        }

        if (ImGuiExt.BeginPopupModal($"Add Bytes##{Name}{address}", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            var size = (ImGui.CalcTextSize("Current Class Size:") * 3) with
            {
                Y = ImGui.GetTextLineHeightWithSpacing() * 10
            };
            var windowSize = ImGui.GetWindowViewport().Size;
            if (windowSize.X < size.X)
                size.X = windowSize.X;
            if (windowSize.Y < size.Y)
                size.Y = windowSize.Y;
            var windowTopLeft = windowSize / 2 - size / 2;
            ImGui.SetWindowPos(windowTopLeft);
            ImGui.SetWindowSize(size);
            ImGui.TextUnformatted("Number of bytes to add:");
            var bytes = _addingCustomBytesSize;
            bool hex = false;
            if (ImGui.InputInt("##BytesToAdd", ref bytes, 1, 100, hex ? ImGuiInputTextFlags.CharsHexadecimal : ImGuiInputTextFlags.CharsDecimal))
            {
                if (bytes < 0)
                    bytes = 0;
                _addingCustomBytesSize = bytes;
            }

            if (ImGui.RadioButton("Decimal", !hex))
            {
                hex = false;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Hex", hex))
            {
                hex = true;
            }

            ImGui.Columns(2, "##Coll", false);
            ImGui.TextUnformatted("Current Class Size:");
            ImGui.NextColumn();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted("0x" + Size.ToString("X"));
            ImGui.SameLine();
            ImGui.TextUnformatted(" / ");
            ImGui.SameLine();
            ImGui.TextUnformatted(Size.ToString());
            ImGui.NextColumn();
            ImGui.TextUnformatted("New Class Size:");
            ImGui.NextColumn();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted("0x" + (Size + bytes).ToString("X"));
            ImGui.SameLine();
            ImGui.TextUnformatted(" / ");
            ImGui.SameLine();
            ImGui.TextUnformatted((Size + bytes).ToString());
            ImGui.NextColumn();
            ImGui.Columns(1);

            if (ImGui.Button("Ok"))
            {
                if (bytes > 0)
                    AddBytes(bytes);
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }

    private void AddBytes(int size)
    {
        while (size > 0)
        {
            switch (size)
            {
                case >= 8:
                    Renderers.Add(new Unknown8Renderer());
                    size -= 8;
                    break;
                case >= 4:
                    Renderers.Add(new Unknown4Renderer());
                    size -= 4;
                    break;
                case >= 2:
                    Renderers.Add(new Unknown2Renderer());
                    size -= 2;
                    break;
                default:
                    Renderers.Add(new Unknown1Renderer());
                    size -= 1;
                    break;
            }
        }
        _height = -1;
    }

    private void InsertBytes(int index, int size)
    {
        while (size > 0)
        {
            switch (size)
            {
                case >= 8:
                    Renderers.Insert(index, new Unknown8Renderer());
                    size -= 8;
                    break;
                case >= 4:
                    Renderers.Insert(index, new Unknown4Renderer());
                    size -= 4;
                    break;
                case >= 2:
                    Renderers.Insert(index, new Unknown2Renderer());
                    size -= 2;
                    break;
                default:
                    Renderers.Insert(index, new Unknown1Renderer());
                    size -= 1;
                    break;
            }
            index++;
        }
        _height = -1;
    }
}

public float GetHeight()
{
    if (_height > 0)
        return _height;
    var height = ImGui.GetTextLineHeightWithSpacing();
    return _height = height + Renderers.Sum(t => t.GetHeight());
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