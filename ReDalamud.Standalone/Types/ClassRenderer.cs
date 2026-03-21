using System.Globalization;

namespace ReDalamud.Standalone.Types;

public class ClassRenderer : IRenderer, IComparable<ClassRenderer>
{
    public bool HasName => true;
    public bool HasCode => true;
    public int Size => Renderers.Sum(t => t.Size);
    public string FieldName { get; set; }
    public List<IRenderer> Renderers = new();
    public bool IsCollapsed;
    // ReSharper disable once MemberInitializerValueIgnored
    public nint Address = unchecked((nint)0x140000000);
    public bool IsPointerAddress;
    public string Name = GenerateRandomName();
    private string SizeString => Config.Global.DisplayAsHex ? "0x" + Size.ToString("X") : Size.ToString();
    private float _height = -1;
    private bool _addingCustomBytes;
    private int _addingCustomBytesSize;
    private int _lastSelectedIndex = -1;
    private int[] _selectedIndexes = [];
    private int _hoveredIndex = -1;
    private int _lastHoveredIndex = -1;
    private bool _insertingCustomBytes;

    public ClassRenderer(int size = 0x40)
    {
        Address = MemoryRead.OpenedProcess.MainModule!.BaseAddress;
        FieldName = Address.ToString("X");
        for (var i = 0; i < size; i += 8)
        {
            Renderers.Add(new Unknown8Renderer());
        }
    }

    public ClassRenderer(nint address, int size = 0x40) : this(size)
    {
        Address = address;
        FieldName = address.ToString("X");
    }

    public void DrawMemory(nint address, int offset)
    {
        if (address == nint.Zero)
            address = Address;
        if (IsPointerAddress && MemoryRead.Read(address, out nint ptr))
        {
            address = Address = ptr;
            IsPointerAddress = false;
        }

        _hoveredIndex = -1;

        DrawHeader(address);
        if (IsCollapsed)
            return;

        var scrollY = ImGui.GetScrollY();
        var windowHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild($"ClassRendererMemory##{Name}{address}", new Vector2(-1, GetHeight()));
        var posY = 0f;
        offset = 0;
        var childSize = new Vector2(-1, ImGui.GetTextLineHeightWithSpacing());
        for (var index = 0; index < Renderers.Count; index++)
        {
            //if (!isVisible(posY))
            //{
            //    posY += renderer.GetHeight();
            //    continue;
            //}
            var renderer = Renderers[index];
            var selected = index == _lastSelectedIndex || _selectedIndexes.Contains(index);
            if (selected)
                ImGui.PushStyleColor(ImGuiCol.ChildBg, (Vector4)Config.Styles.SelectedColor);
            if (index == _lastHoveredIndex)
                ImGui.PushStyleColor(ImGuiCol.ChildBg, (Vector4)Config.Styles.HoveredColor);
            var originalSpacing = ImGui.GetStyle().ItemSpacing.Y;
            ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, 0);
            if (ImGui.BeginChild($"ClassRendererRow###{Name}{address}{index}", childSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, originalSpacing);
                renderer.DrawMemory(address + offset, offset);
                ImGui.PopStyleVar();
            }
            ImGui.EndChild();
            ImGui.PopStyleVar();
            if (selected)
                ImGui.PopStyleColor();
            if (index == _lastHoveredIndex)
                ImGui.PopStyleColor();
            if (ImGui.IsItemHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    var lastIndex = index;
                    if (ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || ImGui.IsKeyDown(ImGuiKey.RightCtrl))
                    {
                        if (_selectedIndexes.Contains(index))
                        {
                            _selectedIndexes = _selectedIndexes.Except([index]).ToArray();
                            lastIndex = -1;
                        }
                        else
                        {
                            _selectedIndexes = [.. _selectedIndexes, index];
                            if(!_selectedIndexes.Contains(_lastSelectedIndex))
                                _selectedIndexes = [.. _selectedIndexes, _lastSelectedIndex];
                        }
                    }
                    else if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || ImGui.IsKeyDown(ImGuiKey.RightShift))
                    {
                        _selectedIndexes.Sort();
                        var minIndex = Math.Min(GetMinIndexSelected, index);
                        var maxIndex = Math.Clamp(Math.Max(GetMaxIndexSelected, index) + 1, 0, index + 1);
                        _selectedIndexes = Enumerable.Range(minIndex, maxIndex - minIndex).ToArray();
                    }
                    else
                    {
                        _selectedIndexes = [];
                    }
                    _lastSelectedIndex = lastIndex;
                }
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    if (!_selectedIndexes.Contains(index))
                    {
                        _selectedIndexes = [index];
                    }
                    _lastSelectedIndex = index;
                    ImGui.OpenPopup($"ClassPopup##{Name}{address}");
                }
                _hoveredIndex = index;
                if(renderer is IUnknownRenderer unknownRenderer)
                    unknownRenderer.DrawToolTip();
            }

            posY += renderer.GetHeight();
            offset += renderer.Size;
        }
        DrawPopUp(address);
        ImGui.EndChild();
        _lastHoveredIndex = _hoveredIndex;
        return;
    }

    private int GetMinIndexSelected => _selectedIndexes.Length > 0 ? _selectedIndexes[0] : _lastSelectedIndex;

    private int GetMaxIndexSelected => _selectedIndexes.Length > 0 ? _selectedIndexes[^1] : _lastSelectedIndex;

    private void DrawHeader(nint address)
    {
        var style = new ImGuiSmrt.Style();
        var color = new ImGuiSmrt.Color();
        if (_lastSelectedIndex == -1)
            color.Push(ImGuiCol.ChildBg, (Vector4)Config.Styles.SelectedColor);
        if (_lastHoveredIndex == -1)
            color.Push(ImGuiCol.ChildBg, (Vector4)Config.Styles.HoveredColor);
        var size = ImGui.GetTextLineHeight();
        ImGui.BeginChild($"ClassHeader##{Name}{address}", new Vector2(-1, size), ImGuiChildFlags.None, ImGuiWindowFlags.NoScrollbar);
        style.Push(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        style.Push(ImGuiStyleVar.FrameRounding, 0);
        color.Push(ImGuiCol.Button, Config.Styles.BackgroundColor);
        if (ImGui.ImageButton($"Collapse##{Name}{address}", IconLoader.GetIconTextureId(IsCollapsed ? Icon16.ClosedIcon : Icon16.OpenIcon), new Vector2(size)))
        {
            IsCollapsed = !IsCollapsed;
        }
        ImGui.SameLine();
        ImGui.Image(IconLoader.GetIconTextureId(Icon16.ClassType), new Vector2(size));
        style.Dispose();
        ImGui.SameLine();
        color.PushTextColor(Config.Styles.AddressColor);
        var changed = "";
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        if (ImGuiExt.InputText($"AddressEdit##{address}", FieldName, ref changed))
        {
            FieldName = changed.Trim();
            if (FieldName[0] is '<')
            {
                // TODO: Get the address from data.yml in directory of Config.Global.ClientStructsPath
            }
            else if (nint.TryParse(FieldName, NumberStyles.AllowHexSpecifier, null, out nint parsedAddress))
                Address = parsedAddress;
            FieldName = Address.ToString("X");
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
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                _selectedIndexes = [];
                _lastSelectedIndex = -1;
            }
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                _selectedIndexes = [];
                _lastSelectedIndex = -1;
                ImGui.OpenPopup($"ClassPopup##{Name}{address}");
            }
        }

        DrawPopUp(address);
    }

    public void DrawPopUp(nint address)
    {
        if (ImGui.BeginPopupContextWindow($"ClassPopup##{Name}{address}"))
        {
            if (ImGui.Selectable("Copy Address"))
            {
                if (_lastSelectedIndex == -1)
                    ImGui.SetClipboardText(Address.ToString("X"));
                else
                    ImGui.SetClipboardText((Renderers[..(_lastSelectedIndex+1)].Sum(t => t.Size) + Address).ToString("X"));
            }

            ImGui.BeginDisabled(_lastSelectedIndex == -1);
            ImGuiExt.MenuWithIcon(Icon16.ExchangeButton, "Change Type", $"ChangeType##{Name}{address}", () =>
            {
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonHex64, "Hex64"))
                    _selectedIndexes = InsertType(_selectedIndexes, new Unknown8Renderer());
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonHex32, "Hex32"))
                    _selectedIndexes = InsertType(_selectedIndexes, new Unknown4Renderer());
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonHex16, "Hex16"))
                    _selectedIndexes = InsertType(_selectedIndexes, new Unknown2Renderer());
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonHex8, "Hex8"))
                    _selectedIndexes = InsertType(_selectedIndexes, new Unknown1Renderer());
                ImGui.Separator();
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonNInt, "NInt"))
                    _selectedIndexes = InsertType(_selectedIndexes, new NintRenderer());
                // int64, int32, int16, int8, ImGui.Seperator()
                // nuint, uint64 / QWORD, uint32 / DWORD, uint16 / WORD, uint8 / BYTE, ImGui.Seperator()
                // bool, bitfield, Enum, , ImGui.Seperator()
                // float, double, ImGui.Seperator()
                // vector4, vector3, vector2, matrix 4x4, matrix 3x4, matrix 3x3, ImGui.Seperator()
                // utf8 / ascii text, utf8 / ascii text pointer, utf16 / unicode text, utf16 / unicode text pointer, ImGui.Seperator()
                // pointer, array, union, ImGui.Seperator()
                // class instance, ImGui.Seperator()
                // vtable pointer, function, function pointer, ImGui.Seperator()
                // custom
            });
            ImGui.EndDisabled();

            ImGuiExt.MenuWithIcon(Icon16.ButtonAddBytesX, "Add Bytes", $"ClassAddBytes##{Name}{address}", () =>
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

            });

            ImGui.BeginDisabled(_lastSelectedIndex == -1);
            ImGuiExt.MenuWithIcon(Icon16.ButtonInsertBytesX, "Insert Bytes", $"ClassInsertBytes##{Name}{address}", () =>
            {
                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonInsertBytes4, "Insert 4 Byte"))
                    InsertBytes(GetMaxIndexSelected, 4);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonInsertBytes8, "Insert 8 Bytes"))
                    InsertBytes(GetMaxIndexSelected, 8);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonInsertBytes64, "Insert 64 Bytes"))
                    InsertBytes(GetMaxIndexSelected, 64);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonInsertBytes256, "Insert 256 Bytes"))
                    InsertBytes(GetMaxIndexSelected, 256);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonInsertBytes1024, "Insert 1024 Bytes"))
                    InsertBytes(GetMaxIndexSelected, 1024);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonInsertBytes4096, "Insert 4096 Bytes"))
                    InsertBytes(GetMaxIndexSelected, 4096);

                if (ImGuiExt.SelectableWithIcon(Icon16.ButtonInsertBytesX, "Insert ... Bytes"))
                    _insertingCustomBytes = true;
            });
            ImGui.EndDisabled();

            ImGui.EndPopup();
        }

        if (_addingCustomBytes)
        {
            ImGui.OpenPopup($"Add Bytes##{Name}{address}");
            _addingCustomBytes = false;
        }

        if (ImGui.BeginPopupModal($"Add Bytes##{Name}{address}", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
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

        if (_insertingCustomBytes)
        {
            ImGui.OpenPopup($"Insert Bytes##{Name}{address}");
            _insertingCustomBytes = false;
        }

        if (ImGui.BeginPopupModal($"Insert Bytes##{Name}{address}", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
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
            var windowTopLeft = (windowSize - size) / 2;
            ImGui.SetWindowPos(windowTopLeft);
            ImGui.SetWindowSize(size);
            ImGui.TextUnformatted("Number of bytes to insert:");
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
                    InsertBytes(GetMaxIndexSelected, bytes);
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

    private void InsertType(int index, IRenderer renderer)
    {
        var sizeLeft = Renderers[index].Size - renderer.Size;
        Renderers[index] = renderer;
        index++;
        while (sizeLeft > 0)
        {
            switch (sizeLeft)
            {
                case >= 8:
                    Renderers.Insert(index, new Unknown8Renderer());
                    sizeLeft -= 8;
                    break;
                case >= 4:
                    Renderers.Insert(index, new Unknown4Renderer());
                    sizeLeft -= 4;
                    break;
                case >= 2:
                    Renderers.Insert(index, new Unknown2Renderer());
                    sizeLeft -= 2;
                    break;
                default:
                    Renderers.Insert(index, new Unknown1Renderer());
                    sizeLeft -= 1;
                    break;
            }
            index++;
        }
    }

    private int[] InsertType(Span<int> indexes, IRenderer renderer)
    {
        if (indexes.Length == 1)
        {
            InsertType(indexes[0], renderer);
            return [indexes[0]];
        }
        indexes.Sort();
        var firstIndex = indexes[0];
        var lastIndex = indexes[^1] + 1;
        var totalSize = Renderers[firstIndex..lastIndex].Sum(t => t.Size);
        Renderers.RemoveRange(firstIndex, lastIndex - firstIndex);
        var count = (int)Math.Ceiling(totalSize / (float)renderer.Size);
        for (var i = 0; i < count; i++)
        {
            Renderers.Insert(firstIndex, renderer);
        }
        return Enumerable.Range(firstIndex, count).ToArray();
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

        return string.Compare(FieldName, other.FieldName, StringComparison.Ordinal);
    }
}