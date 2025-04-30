using SDLEvent = Hexa.NET.SDL3.SDLEvent;

namespace ReDalamud.Standalone.Windows;

public class MainMenuBar
{
    public static bool StyleWindowOpened;
    
    public static void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            var io = ImGui.GetIO();
            if (ImGui.BeginMenu("File"))
            {
                if(ImGui.MenuItem("Exit"))
                {
                    var quitEvent = new SDLEvent
                    {
                        Type = (int)SDLEventType.Quit
                    };
                    SDL.PushEvent(ref quitEvent);
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
                if (ImGui.MenuItem("Style", "", StyleWindowOpened))
                {
                    StyleWindowOpened = !StyleWindowOpened;
                }
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }
}