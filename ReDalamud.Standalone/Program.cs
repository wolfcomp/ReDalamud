using System.Reflection;
using ImGuiNET;
using ReDalamud.Standalone.Resources;
using SDL2;

namespace ReDalamud.Standalone;

public class Program
{
    private static ImGuiRenderer _renderer = null!;
    public static bool IsFirstSetup = true;

    public static void Main(string[] args)
    {
        var loc = Directory.GetCurrentDirectory();
        var file = Path.Combine(loc, "imgui.ini");
        IsFirstSetup = !File.Exists(file);

        _renderer = ImGuiRenderer.CreateWindowAndGlContext("ReDalamud.Standalone", 800, 600);

        var quit = false;

        while (!quit)
        {
            while(SDL.SDL_PollEvent(out var e) != 0)
            {
                _renderer.ProcessEvent(e);
                switch(e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        quit = true;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                quit = true;
                                break;
                        }

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
            _renderer.Render();
            SDL.SDL_GL_SwapWindow(_renderer.Window);
        }

        SDL.SDL_GL_DeleteContext(_renderer.GlContext);
        SDL.SDL_DestroyWindow(_renderer.Window);
        SDL.SDL_Quit();
        IconLoader.Dispose();
    }
}