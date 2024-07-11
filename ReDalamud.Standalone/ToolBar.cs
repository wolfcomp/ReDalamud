using ImGuiNET;

namespace ReDalamud.Standalone;

public class ToolBar
{
    public static void Draw()
    {
        ImGui.Begin("ToolBar");
        if (Program.IsFirstSetup && DockWindow.DockSpaceId.HasValue)
        {
            ImGui.SetNextWindowDockID(DockWindow.DockSpaceId.Value, ImGuiCond.Once);
        }
        ImGui.Text("testing");
        ImGui.End();
    }
}