namespace ReDalamud.Standalone.Windows;
internal class StaticClassView
{
    public static ClassView? CurrentClassView;

    public static void Draw()
    {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, (Vector4)Config.Styles.BackgroundColor);
        ImGui.Begin("StaticClassView");
        CurrentClassView?.Draw(true);
        ImGui.End();
        ImGui.PopStyleColor();
    }
}
