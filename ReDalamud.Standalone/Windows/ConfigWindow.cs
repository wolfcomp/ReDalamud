using System.Runtime.CompilerServices;

namespace ReDalamud.Standalone.Windows;
public class ConfigWindow
{
    public static unsafe void Draw()
    {
        if(!MainMenuBar.StyleWindowOpened) return;
        ImGui.Begin("ConfigWindow"u8, ref MainMenuBar.StyleWindowOpened, ImGuiWindowFlags.AlwaysVerticalScrollbar);

        if (ImGui.BeginTabBar("StyleTabBar"u8))
        {
            if (ImGui.BeginTabItem("Colors"u8))
            {
                DrawColorPickerOption("Background Color"u8, ref Config.Styles.BackgroundColor);
                DrawColorPickerOption("Selected Color"u8, ref Config.Styles.SelectedColor);
                DrawColorPickerOption("Hovered Color"u8, ref Config.Styles.HoveredColor);
                DrawColorPickerOption("Hidden Color"u8, ref Config.Styles.HiddenColor);
                DrawColorPickerOption("Address Color"u8, ref Config.Styles.AddressColor);
                DrawColorPickerOption("Index Color"u8, ref Config.Styles.IndexColor);
                DrawColorPickerOption("Offset Color"u8, ref Config.Styles.OffsetColor);
                DrawColorPickerOption("VTable Color"u8, ref Config.Styles.VTableColor);
                DrawColorPickerOption("Hex Value Color"u8, ref Config.Styles.HexValueColor);
                DrawColorPickerOption("Comment Color"u8, ref Config.Styles.CommentColor);
                DrawColorPickerOption("Type Color"u8, ref Config.Styles.TypeColor);
                DrawColorPickerOption("Text Color"u8, ref Config.Styles.TextColor);
                DrawColorPickerOption("Name Color"u8, ref Config.Styles.NameColor);
                DrawColorPickerOption("Plugin Info Color"u8, ref Config.Styles.PluginInfoColor);
                DrawColorPickerOption("Value Color"u8, ref Config.Styles.ValueColor);
                ImGui.EndTabItem();
            }

            // if (ImGui.BeginTabItem("Fonts"))
            // {
            //     ImGui.Text("Fonts");
            //     ImGui.EndTabItem();
            // }
            //

            if (ImGui.BeginTabItem("Global"u8))
            {
                ImGui.InputText("ClientStructs IDA Location"u8, ref Config.Global.ClientStructsPath, 1000);
                if (ImGui.RadioButton("Size as Hex"u8, Config.Global.DisplayAsHex))
                {
                    Config.Global.DisplayAsHex = !Config.Global.DisplayAsHex;
                }
                if (ImGui.RadioButton("Show field name on unknowns"u8, Config.Global.ShowNameOnUnknown))
                {
                    Config.Global.ShowNameOnUnknown = !Config.Global.ShowNameOnUnknown;
                }
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("ImGui Style"u8))
            {
                ImGui.ShowStyleEditor();
                ImGui.EndTabItem();
            }

            if (ImGui.Button("Save"u8))
                ShouldSaveOnFrame = true;

            ImGui.EndTabBar();
        }

        ImGui.End();
    }

    private static unsafe void DrawColorPickerOption(ImUtf8String name, ref Color color)
    {
        var internalColor = (float*)Unsafe.AsPointer(ref color.InternalValue);
        if (ImGui.ColorEdit4(name, internalColor, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.DisplayRgb))
            color.InternalValue = Unsafe.AsRef<Vector4>(internalColor);
    }
}
