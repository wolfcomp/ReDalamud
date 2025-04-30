using ReDalamud.Standalone.Types;
using SDLEvent = Hexa.NET.SDL3.SDLEvent;

namespace ReDalamud.Standalone;

public class Program
{
    private static ImGuiRenderer _renderer = null!;
    private static Timer? _timer;
    public static Random Rand = new();
    public static bool ShouldSaveOnFrame = false;
    public static unsafe void Main(string[] args)
    {
        var loc = Directory.GetCurrentDirectory();
        DockWindow.IsFirstSetup = !File.Exists(Path.Combine(loc, "imgui.ini"));

        _renderer = ImGuiRenderer.CreateWindowAndGlContext("ReDalamud.Standalone", 800, 600);
        // Config.ImGuiStyle = ImGui.GetStyle();
        Config.Load();

        var quit = false;

        var process = MemoryRead.OpenProcess("ffxiv_dx11");

        if (process != nint.Zero)
        {
            Console.WriteLine("Opened process successfully");
            StaticClassView.CurrentClassView = new ClassRenderer(MemoryRead.GetOpenedProcessAddress());
            _timer = new Timer(_ => MemoryRead.ScanAllProcessMemoryRegions(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            Task.Run(ClassList.InitTypes);
        }
        else
        {
            Console.WriteLine("Failed to open process");
        }

        var imGuiCols = Enum.GetValues<ImGuiCol>().Where(t => t != ImGuiCol.Count).ToList();
        var imGuiStyleVars = Enum.GetValues<ImGuiStyleVar>().Where(t => t != ImGuiStyleVar.Count).ToList();
        foreach (var imGuiCol in imGuiCols)
        {
            unsafe
            {
                var col = ImGui.GetStyleColorVec4(imGuiCol);
                ImGuiSmrt.Color.Stack.Add((imGuiCol, *col));
            }
        }

        foreach (var imGuiStyleVar in imGuiStyleVars)
        {
            var vec = ImGuiExt.GetStyleObject(imGuiStyleVar);
            ImGuiSmrt.Style.Stack.Add((imGuiStyleVar, vec));
        }

        SDLEvent sdlEvent = default;

        while (!quit)
        {
            SDL.PumpEvents();
            while (SDL.PollEvent(ref sdlEvent))
            {
                ImGuiImplSDL3.SDL3ProcessEvent((Hexa.NET.ImGui.Backends.SDL3.SDLEvent*)&sdlEvent);
                quit = (SDLEventType)sdlEvent.Type switch
                {
                    SDLEventType.Quit or SDLEventType.Terminating => true,
                    SDLEventType.WindowCloseRequested => sdlEvent.Window.WindowID == _renderer.WindowId,
                    _ => quit
                };
            }

            _renderer.Clear(0.05f, 0.05f, 0.05f, 1f);
            _renderer.NewFrame();
            MainMenuBar.Draw();
            DockWindow.Draw();
            ToolBar.Draw();
            ClassList.Draw();
            EnumList.Draw();
            StaticClassView.Draw();
            ConfigWindow.Draw();
            _renderer.Render();

            if (ShouldSaveOnFrame)
            {
                Config.Save();
                ShouldSaveOnFrame = false;
            }
        }

        ImGuiImplOpenGL3.Shutdown();
        ImGuiImplSDL3.SDL3Shutdown();
        ImGui.DestroyContext();
        _renderer.Dispose();
        _timer?.Dispose();
        IconLoader.Dispose();
        MemoryRead.Dispose();
        SDL.Quit();
    }
}