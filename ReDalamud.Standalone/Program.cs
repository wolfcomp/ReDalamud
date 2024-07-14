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
            Task.Run(ClassList.InitTypes);
        }
        else
        {
            Console.WriteLine("Failed to open process");
        }

        var imGuiCols = Enum.GetValues<ImGuiCol>().Where(t => t != ImGuiCol.COUNT).ToList();
        var imGuiStyleVars = Enum.GetValues<ImGuiStyleVar>().Where(t => t != ImGuiStyleVar.COUNT).ToList();
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