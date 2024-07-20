using ImGui = ImGuiNET.ImGui;

namespace ReDalamud.Standalone.Windows;
public class ConfigWindow
{
    public static unsafe void Draw()
    {
        if(!MainMenuBar.StyleWindowOpened) return;
        ImGui.Begin("ConfigWindow", ref MainMenuBar.StyleWindowOpened, ImGuiWindowFlags.AlwaysAutoResize);

        if (ImGui.BeginTabBar("StyleTabBar"))
        {
            if (ImGui.BeginTabItem("Colors"))
            {
                DrawColorPickerOption("Background Color", ref Config.Styles.BackgroundColor);
                DrawColorPickerOption("Selected Color", ref Config.Styles.SelectedColor);
                DrawColorPickerOption("Hidden Color", ref Config.Styles.HiddenColor);
                DrawColorPickerOption("Address Color", ref Config.Styles.AddressColor);
                DrawColorPickerOption("Index Color", ref Config.Styles.IndexColor);
                DrawColorPickerOption("Offset Color", ref Config.Styles.OffsetColor);
                DrawColorPickerOption("VTable Color", ref Config.Styles.VTableColor);
                DrawColorPickerOption("Hex Value Color", ref Config.Styles.HexValueColor);
                DrawColorPickerOption("Comment Color", ref Config.Styles.CommentColor);
                DrawColorPickerOption("Type Color", ref Config.Styles.TypeColor);
                DrawColorPickerOption("Text Color", ref Config.Styles.TextColor);
                DrawColorPickerOption("Name Color", ref Config.Styles.NameColor);
                DrawColorPickerOption("Plugin Info Color", ref Config.Styles.PluginInfoColor);
                DrawColorPickerOption("Value Color", ref Config.Styles.ValueColor);
                ImGui.EndTabItem();
            }

            // if (ImGui.BeginTabItem("Fonts"))
            // {
            //     ImGui.Text("Fonts");
            //     ImGui.EndTabItem();
            // }
            //

            if (ImGui.BeginTabItem("Global"))
            {
                ImGui.InputText("ClientStructs IDA Location", ref Config.Global.ClientStructsPath, 1000);
                if (ImGui.RadioButton("Size as Hex", Config.Global.DisplayAsHex))
                {
                    Config.Global.DisplayAsHex = !Config.Global.DisplayAsHex;
                }
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("ImGui Style"))
            {
                ImGui.ShowStyleEditor();
                ImGui.EndTabItem();
            }

            if (ImGui.Button("Save"))
                ShouldSaveOnFrame = true;

            ImGui.EndTabBar();
        }

        ImGui.End();
    }

    private static void DrawColorPickerOption(string name, ref Color color)
    {
        var internalColor = (Vector4)color;
        if (ImGui.ColorEdit4(name, ref internalColor, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.DisplayRGB))
            color = internalColor;
    }
}
