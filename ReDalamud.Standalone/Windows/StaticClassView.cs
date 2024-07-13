using ReDalamud.Standalone.Types;

namespace ReDalamud.Standalone.Windows;
internal class StaticClassView
{
    public static ClassRenderer? CurrentClassView;

    public static void Draw()
    {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, (Vector4)Config.Styles.BackgroundColor);
        ImGui.Begin("StaticClassView");
        CurrentClassView?.DrawMemory(nint.Zero);
        ImGui.End();
        ImGui.PopStyleColor();
    }
}
