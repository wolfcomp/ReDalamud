using ReDalamud.Standalone.Types;

namespace ReDalamud.Standalone;

public class Program
{
    private static ImGuiRenderer _renderer = null!;
    private static Timer? _timer;
    public static Random Rand = new();
    public static void Main(string[] args)
    {
        var loc = Directory.GetCurrentDirectory();
        DockWindow.IsFirstSetup = !File.Exists(Path.Combine(loc, "imgui.ini"));
        Config.Load();

        _renderer = ImGuiRenderer.CreateWindowAndGlContext("ReDalamud.Standalone", 800, 600);

        var quit = false;

        var process = MemoryRead.OpenProcess("ffxiv_dx11");

        if(process != nint.Zero)
        {
            Console.WriteLine("Opened process successfully");
            StaticClassView.CurrentClassView = new ClassRenderer(MemoryRead.GetOpenedProcessAddress());
            _timer = new Timer(_ => MemoryRead.ScanAllProcessMemoryRegions(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
        else
        {
            Console.WriteLine("Failed to open process");
        }

        while (!quit)
        {
            while(SDL_PollEvent(out var e) != 0)
            {
                _renderer.ProcessEvent(e);
                switch(e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        quit = true;
                        break;
                }
            }

            _renderer.ClearColor(0.05f, 0.05f, 0.05f, 1f);
            _renderer.NewFrame();
            MainMenuBar.Draw();
            DockWindow.Draw();
            ToolBar.Draw();
            ClassList.Draw();
            EnumList.Draw();
            StaticClassView.Draw();
            ConfigWindow.Draw();
            _renderer.Render();
            SDL_GL_SwapWindow(_renderer.Window);
        }

        SDL_GL_DeleteContext(_renderer.GlContext);
        SDL_DestroyWindow(_renderer.Window);
        SDL_Quit();
        _timer?.Dispose();
        IconLoader.Dispose();
        MemoryRead.Dispose();
    }
}