using System.ComponentModel;
using System.Text;

namespace ReDalamud.Standalone;
public static unsafe class Config
{
    public static Styles Styles = new();
    public static Global Global = new();
    public static ImGuiStyle* ImGuiStyle;

    public static void Save()
    {
        var sb = new StringBuilder();
        Save(typeof(ImGuiStyle), *ImGuiStyle, sb);
        Save(typeof(Styles), Styles, sb);
        Save(typeof(Global), Global, sb);
        File.WriteAllText("config.ini", sb.ToString());
    }

    private static void Save(Type type, object obj, StringBuilder sb)
    {
        var types = new HashSet<Type>();
        sb.AppendLine($"[{type.Name}]");
        foreach (var field in type.GetFields().OrderBy(t => t.FieldType.Name))
        {
            if (types.Add(field.FieldType))
            {
                var doc = field.FieldType.GetDocString();
                if (!string.IsNullOrWhiteSpace(doc))
                    sb.AppendLine($"{doc}");
            }
            var value = field.GetValue(obj);
            if (value == null)
                continue;
            sb.AppendLine($"{field.Name}={value.ToIniString()}");
        }

        sb.AppendLine();
    }

    public static void Load()
    {
        if (!File.Exists("config.ini"))
            return;
        // Load the config from an ini file
        var lines = File.ReadAllLines("config.ini").ToList();

        var grouped = new Dictionary<string, List<(string Key, string Value)>>();
        var currentGroup = string.Empty;
        foreach (var line in lines)
        {
            if (line.StartsWith('['))
            {
                currentGroup = line.Trim('[', ']');
                grouped[currentGroup] = new List<(string Key, string Value)>();
            }
            else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith('#'))
            {
                var split = line.Split('=');
                grouped[currentGroup].Add((split[0], split[1]));
            }
        }

        grouped.TryGetValue("Styles", out var styleLines);
        if (styleLines != null)
            foreach (var (key, value) in styleLines)
                Load(Styles, key, value);
        grouped.TryGetValue("Global", out var globalLines);
        if (globalLines != null)
            foreach (var (key, value) in globalLines)
                Load(Global, key, value);
        grouped.TryGetValue("ImGuiStyle", out var defaultStyle);
        if (defaultStyle != null)
        {
            var style = new Dictionary<string, object>();
            foreach (var field in ImGuiStyle->GetType().GetFields())
            {
                var value = defaultStyle.FirstOrDefault(t => t.Key == field.Name).Value;
                if (value == null)
                    continue;
                style[field.Name] = field.FieldType.FromIniString(value);
                if (style[field.Name].GetType() != field.FieldType)
                {
                    if(field.FieldType.IsEnum)
                        style[field.Name] = Enum.Parse(field.FieldType, value);
                    else
                        style[field.Name] = Convert.ChangeType(style[field.Name], field.FieldType);
                }
            }
            SetStylePtr(style);
        }
    }

    private static void Load(object obj, string key, string value)
    {
        var field = obj.GetType().GetField(key);
        if (field == null)
            return;
        var val = field.FieldType.FromIniString(value);
        try
        {
            field.SetValue(obj, Convert.ChangeType(val, field.FieldType));
        }
        catch
        {
            field.SetValue(obj, TypeDescriptor.GetConverter(field.FieldType).ConvertFromString(null, null, value));
        }
    }

    private static void SetStylePtr(Dictionary<string, object> style)
    {
        var stylePtr = ImGui.GetStyle().NativePtr;
        stylePtr->Alpha = (float)style["Alpha"];
        stylePtr->DisabledAlpha = (float)style["DisabledAlpha"];
        stylePtr->WindowPadding = (Vector2)style["WindowPadding"];
        stylePtr->WindowRounding = (float)style["WindowRounding"];
        stylePtr->WindowBorderSize = (float)style["WindowBorderSize"];
        stylePtr->WindowMinSize = (Vector2)style["WindowMinSize"];
        stylePtr->WindowTitleAlign = (Vector2)style["WindowTitleAlign"];
        stylePtr->WindowMenuButtonPosition = (ImGuiDir)style["WindowMenuButtonPosition"];
        stylePtr->ChildRounding = (float)style["ChildRounding"];
        stylePtr->ChildBorderSize = (float)style["ChildBorderSize"];
        stylePtr->PopupRounding = (float)style["PopupRounding"];
        stylePtr->PopupBorderSize = (float)style["PopupBorderSize"];
        stylePtr->FramePadding = (Vector2)style["FramePadding"];
        stylePtr->FrameRounding = (float)style["FrameRounding"];
        stylePtr->FrameBorderSize = (float)style["FrameBorderSize"];
        stylePtr->ItemSpacing = (Vector2)style["ItemSpacing"];
        stylePtr->ItemInnerSpacing = (Vector2)style["ItemInnerSpacing"];
        stylePtr->CellPadding = (Vector2)style["CellPadding"];
        stylePtr->TouchExtraPadding = (Vector2)style["TouchExtraPadding"];
        stylePtr->IndentSpacing = (float)style["IndentSpacing"];
        stylePtr->ColumnsMinSpacing = (float)style["ColumnsMinSpacing"];
        stylePtr->ScrollbarSize = (float)style["ScrollbarSize"];
        stylePtr->ScrollbarRounding = (float)style["ScrollbarRounding"];
        stylePtr->GrabMinSize = (float)style["GrabMinSize"];
        stylePtr->GrabRounding = (float)style["GrabRounding"];
        stylePtr->LogSliderDeadzone = (float)style["LogSliderDeadzone"];
        stylePtr->TabRounding = (float)style["TabRounding"];
        stylePtr->TabBorderSize = (float)style["TabBorderSize"];
        stylePtr->TabMinWidthForCloseButton = (float)style["TabMinWidthForCloseButton"];
        stylePtr->ColorButtonPosition = (ImGuiDir)style["ColorButtonPosition"];
        stylePtr->ButtonTextAlign = (Vector2)style["ButtonTextAlign"];
        stylePtr->SelectableTextAlign = (Vector2)style["SelectableTextAlign"];
        stylePtr->SeparatorTextBorderSize = (float)style["SeparatorTextBorderSize"];
        stylePtr->SeparatorTextAlign = (Vector2)style["SeparatorTextAlign"];
        stylePtr->SeparatorTextPadding = (Vector2)style["SeparatorTextPadding"];
        stylePtr->DisplayWindowPadding = (Vector2)style["DisplayWindowPadding"];
        stylePtr->DisplaySafeAreaPadding = (Vector2)style["DisplaySafeAreaPadding"];
        stylePtr->DockingSeparatorSize = (float)style["DockingSeparatorSize"];
        stylePtr->MouseCursorScale = (float)style["MouseCursorScale"];
        stylePtr->AntiAliasedLines = (byte)style["AntiAliasedLines"];
        stylePtr->AntiAliasedLinesUseTex = (byte)style["AntiAliasedLinesUseTex"];
        stylePtr->AntiAliasedFill = (byte)style["AntiAliasedFill"];
        stylePtr->CurveTessellationTol = (float)style["CurveTessellationTol"];
        stylePtr->CircleTessellationMaxError = (float)style["CircleTessellationMaxError"];
        stylePtr->Colors_0 = (Vector4)style["Colors_0"];
        stylePtr->Colors_1 = (Vector4)style["Colors_1"];
        stylePtr->Colors_2 = (Vector4)style["Colors_2"];
        stylePtr->Colors_3 = (Vector4)style["Colors_3"];
        stylePtr->Colors_4 = (Vector4)style["Colors_4"];
        stylePtr->Colors_5 = (Vector4)style["Colors_5"];
        stylePtr->Colors_6 = (Vector4)style["Colors_6"];
        stylePtr->Colors_7 = (Vector4)style["Colors_7"];
        stylePtr->Colors_8 = (Vector4)style["Colors_8"];
        stylePtr->Colors_9 = (Vector4)style["Colors_9"];
        stylePtr->Colors_10 = (Vector4)style["Colors_10"];
        stylePtr->Colors_11 = (Vector4)style["Colors_11"];
        stylePtr->Colors_12 = (Vector4)style["Colors_12"];
        stylePtr->Colors_13 = (Vector4)style["Colors_13"];
        stylePtr->Colors_14 = (Vector4)style["Colors_14"];
        stylePtr->Colors_15 = (Vector4)style["Colors_15"];
        stylePtr->Colors_16 = (Vector4)style["Colors_16"];
        stylePtr->Colors_17 = (Vector4)style["Colors_17"];
        stylePtr->Colors_18 = (Vector4)style["Colors_18"];
        stylePtr->Colors_19 = (Vector4)style["Colors_19"];
        stylePtr->Colors_20 = (Vector4)style["Colors_20"];
        stylePtr->Colors_21 = (Vector4)style["Colors_21"];
        stylePtr->Colors_22 = (Vector4)style["Colors_22"];
        stylePtr->Colors_23 = (Vector4)style["Colors_23"];
        stylePtr->Colors_24 = (Vector4)style["Colors_24"];
        stylePtr->Colors_25 = (Vector4)style["Colors_25"];
        stylePtr->Colors_26 = (Vector4)style["Colors_26"];
        stylePtr->Colors_27 = (Vector4)style["Colors_27"];
        stylePtr->Colors_28 = (Vector4)style["Colors_28"];
        stylePtr->Colors_29 = (Vector4)style["Colors_29"];
        stylePtr->Colors_30 = (Vector4)style["Colors_30"];
        stylePtr->Colors_31 = (Vector4)style["Colors_31"];
        stylePtr->Colors_32 = (Vector4)style["Colors_32"];
        stylePtr->Colors_33 = (Vector4)style["Colors_33"];
        stylePtr->Colors_34 = (Vector4)style["Colors_34"];
        stylePtr->Colors_35 = (Vector4)style["Colors_35"];
        stylePtr->Colors_36 = (Vector4)style["Colors_36"];
        stylePtr->Colors_37 = (Vector4)style["Colors_37"];
        stylePtr->Colors_38 = (Vector4)style["Colors_38"];
        stylePtr->Colors_39 = (Vector4)style["Colors_39"];
        stylePtr->Colors_40 = (Vector4)style["Colors_40"];
        stylePtr->Colors_41 = (Vector4)style["Colors_41"];
        stylePtr->Colors_42 = (Vector4)style["Colors_42"];
        stylePtr->Colors_43 = (Vector4)style["Colors_43"];
        stylePtr->Colors_44 = (Vector4)style["Colors_44"];
        stylePtr->Colors_45 = (Vector4)style["Colors_45"];
        stylePtr->Colors_46 = (Vector4)style["Colors_46"];
        stylePtr->Colors_47 = (Vector4)style["Colors_47"];
        stylePtr->Colors_48 = (Vector4)style["Colors_48"];
        stylePtr->Colors_49 = (Vector4)style["Colors_49"];
        stylePtr->Colors_50 = (Vector4)style["Colors_50"];
        stylePtr->Colors_51 = (Vector4)style["Colors_51"];
        stylePtr->Colors_52 = (Vector4)style["Colors_52"];
        stylePtr->Colors_53 = (Vector4)style["Colors_53"];
        stylePtr->Colors_54 = (Vector4)style["Colors_54"];
        stylePtr->HoverStationaryDelay = (float)style["HoverStationaryDelay"];
        stylePtr->HoverDelayShort = (float)style["HoverDelayShort"];
        stylePtr->HoverDelayNormal = (float)style["HoverDelayNormal"];
        stylePtr->HoverFlagsForTooltipMouse = (ImGuiHoveredFlags)style["HoverFlagsForTooltipMouse"];
        stylePtr->HoverFlagsForTooltipNav = (ImGuiHoveredFlags)style["HoverFlagsForTooltipNav"];
    }
}

public class Styles
{
    public Color BackgroundColor = Color.FromHex("#373737");
    public Color SelectedColor = Color.FromHex("#707070");
    public Color HiddenColor = Color.FromHex("#6A6A6A");
    public Color AddressColor = Color.FromHex("#00C800");
    public Color IndexColor = Color.FromHex("#20C8C8");
    public Color OffsetColor = Color.FromHex("#FF0000");
    public Color VTableColor = Color.FromHex("#00FF00");
    public Color HexValueColor = Color.FromHex("#E4E4E4");
    public Color CommentColor = Color.FromHex("#00C800");
    public Color TypeColor = Color.FromHex("#8282FF");
    public Color TextColor = Color.FromHex("#3E8BFF");
    public Color NameColor = Color.FromHex("#91E6E6");
    public Color PluginInfoColor = Color.FromHex("#FF00FF");
    public Color ValueColor = Color.FromHex("#FF8000");
}

public class Global
{
    // public bool IsLittleEndian = BitConverter.IsLittleEndian;
    public string ClientStructsPath = "";
    public bool DisplayAsHex = false;
}
