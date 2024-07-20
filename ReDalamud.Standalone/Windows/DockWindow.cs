namespace ReDalamud.Standalone.Windows;

public class DockWindow
{
    public static bool IsFirstSetup = true;
    public static bool Fullscreen = true;
    public static bool Padding = false;
    public static ImGuiDockNodeFlags Flags = ImGuiDockNodeFlags.None;
    public static uint? DockSpaceId;
    public static uint? MainDockSpaceId;

    public static unsafe void Draw()
    {
        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        if (Fullscreen)
        {
            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            windowFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        }
        else
        {
            Flags &= ~ImGuiDockNodeFlags.PassthruCentralNode;
        }

        if ((Flags & ImGuiDockNodeFlags.PassthruCentralNode) == ImGuiDockNodeFlags.PassthruCentralNode)
            windowFlags |= ImGuiWindowFlags.NoBackground;

        if (!Padding)
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));

        ImGui.Begin("DockSpace", windowFlags);
        if (!Padding)
            ImGui.PopStyleVar();

        if (Fullscreen)
            ImGui.PopStyleVar(2);

        var io = ImGui.GetIO();
        if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))
        {
            DockSpaceId = ImGui.GetID("DockSpace");
            ImGui.DockSpace(DockSpaceId.Value, new Vector2(0, 0), Flags);
            if (IsFirstSetup)
            {
                IsFirstSetup = false;
                
                ImGui.Begin("ToolBar");
                ImGui.End();
                ImGui.Begin("ClassList");
                ImGui.End();
                ImGui.Begin("EnumList");
                ImGui.End();
                ImGui.Begin("StaticClassView");
                ImGui.End();

                ImGuiExt.igDockBuilderRemoveNode(DockSpaceId.Value);
                ImGuiExt.igDockBuilderAddNode(DockSpaceId.Value, Flags);
                ImGuiExt.igDockBuilderSetNodeSize(DockSpaceId.Value, ImGui.GetMainViewport().Size);
                uint toolBarAreaId;
                uint bottomAreaId;
                ImGuiExt.igDockBuilderSplitNode(DockSpaceId.Value, ImGuiDir.Up, 0.2f, &toolBarAreaId, &bottomAreaId);
                uint leftAreaId;
                uint classViewAreaId;
                ImGuiExt.igDockBuilderSplitNode(bottomAreaId, ImGuiDir.Left, 0.2f, &leftAreaId, &classViewAreaId);
                uint classListId;
                uint enumListId;
                ImGuiExt.igDockBuilderSplitNode(leftAreaId, ImGuiDir.Up, 0.5f, &classListId, &enumListId);
                ImGuiExt.DockBuilderDockWindow("ToolBar", toolBarAreaId);
                ImGuiExt.DockBuilderDockWindow("ClassList", classListId);
                ImGuiExt.DockBuilderDockWindow("EnumList", enumListId);
                ImGuiExt.DockBuilderDockWindow("StaticClassView", classViewAreaId);
                MainDockSpaceId = classViewAreaId;
                ImGuiExt.igDockBuilderFinish(DockSpaceId.Value);
            }
        }
        else
        {
            ImGui.Text("ERROR: Docking is not enabled! See Demo > Configuration.");
        }

        ImGui.End();
    }
}