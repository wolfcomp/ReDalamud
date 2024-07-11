using ImGuiNET;

namespace ReDalamud.Standalone;
internal class StaticClassView
{
    public static ClassView? CurrentClassView;

    public static void Draw()
    {
        ImGui.Begin("StaticClassView");
        CurrentClassView?.Draw(true);
        ImGui.End();
    }
}
