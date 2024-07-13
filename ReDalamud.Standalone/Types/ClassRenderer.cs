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

    public ClassRenderer()
    {
        Address = MemoryRead.OpenedProcess.MainModule!.BaseAddress;
        OffsetText = Address.ToString("X");
        // Maybe make the default size configurable?
        for (var i = 0; i < 0x40; i+=8)
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

        ImGui.Indent();
        foreach (var (offset, renderer) in Renderers)
        {
            ImGuiExt.PushTextStyleColor(Config.Styles.OffsetColor);
            ImGui.Text($"{offset:X4}");
            ImGui.PopStyleColor();
            ImGui.SameLine();
            renderer.DrawMemory(address + offset);
        }
        ImGui.Unindent();
    }

    private void DrawHeader(nint address)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        ImGui.PushStyleColor(ImGuiCol.Button, (Vector4)Config.Styles.BackgroundColor);
        if (ImGui.ImageButton($"ClassViewCollapse##{Name}{address}", IconLoader.GetIconTextureId(IsCollapsed ? Icon16.ClosedIcon : Icon16.OpenIcon), new Vector2(16)))
        {
            IsCollapsed = !IsCollapsed;
        }
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGui.Image(IconLoader.GetIconTextureId(Icon16.ClassType), new Vector2(16));
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.AddressColor);
        var changed = "";
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        if (ImGuiExt.InputText($"AddressEdit##{address}", OffsetText, ref changed))
        {
            OffsetText = changed.Trim();
            if (OffsetText[0] is '<' or '[')
            {
                // TODO: Get the address from data.yml in directory of Config.Global.ClientStructsPath
            }
            else if(nint.TryParse(OffsetText, NumberStyles.AllowHexSpecifier, null, out nint parsedAddress))
                Address = parsedAddress;
            OffsetText = Address.ToString("X");
        }
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.TypeColor);
        ImGui.Text("Class");
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.NameColor);
        ImGui.PushItemWidth(ImGui.CalcTextSize(string.IsNullOrWhiteSpace(Name) ? "." : Name).X);
        if (ImGuiExt.InputText($"NameEdit##{address}", Name, ref changed))
        {
            Name = changed.Trim();
        }
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGuiExt.PushTextStyleColor(Config.Styles.ValueColor);
        ImGui.TextUnformatted($"[{SizeString}]");
        ImGui.PopStyleColor();
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