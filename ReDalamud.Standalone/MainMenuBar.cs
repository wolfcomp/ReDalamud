using ImGuiNET;
using SDL2;

namespace ReDalamud.Standalone;

public class MainMenuBar
{
    public static void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            var io = ImGui.GetIO();
            if (ImGui.BeginMenu("File"))
            {
                if(ImGui.MenuItem("Exit"))
                {
                    var quitEvent = new SDL.SDL_Event
                    {
                        type = SDL.SDL_EventType.SDL_QUIT
                    };
                    SDL.SDL_PushEvent(ref quitEvent);
                }

                if (ImGui.MenuItem("Open", "Ctrl+O"))
                {
                    // Open file dialog
                }
                if (ImGui.MenuItem("Save", "Ctrl+S"))
                {
                    // Save file dialog
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Config"))
            {
                
                if (ImGui.MenuItem("Docking", "", io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable)))
                {
                    io.ConfigFlags ^= ImGuiConfigFlags.DockingEnable;
                }
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }
}