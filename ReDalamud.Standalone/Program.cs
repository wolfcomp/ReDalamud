namespace ReDalamud.Standalone;

public class Program
{
    private static ImGuiRenderer _renderer = null!;
    public static Random Rand = new();
    public static void Main(string[] args)
    {
        var loc = Directory.GetCurrentDirectory();
        var file = Path.Combine(loc, "imgui.ini");
        DockWindow.IsFirstSetup = !File.Exists(file);

        _renderer = ImGuiRenderer.CreateWindowAndGlContext("ReDalamud.Standalone", 800, 600);

        var quit = false;

        var process = MemoryRead.OpenProcess("ffxiv_dx11");

        if(process != nint.Zero)
        {
            Console.WriteLine("Opened process successfully");
            StaticClassView.CurrentClassView = new ClassView(MemoryRead.GetOpenedProcessAddress());
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
            StyleWindow.Draw();
            _renderer.Render();
            SDL_GL_SwapWindow(_renderer.Window);
        }

        SDL_GL_DeleteContext(_renderer.GlContext);
        SDL_DestroyWindow(_renderer.Window);
        SDL_Quit();
        IconLoader.Dispose();
        MemoryRead.Dispose();
    }
}