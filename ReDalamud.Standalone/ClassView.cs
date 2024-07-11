using ImGuiNET;

namespace ReDalamud.Standalone;
public class ClassView
{
    public nint Offset;
    public string Name;

    public ClassView(nint offset)
    {
        Offset = offset;
    }

    public void Draw(bool isStatic)
    {
        if (!isStatic)
            ImGui.Begin($"ClassView##{Offset}");

        ImGui.Text(Name);

        if (!isStatic)
            ImGui.End();
    }
}
