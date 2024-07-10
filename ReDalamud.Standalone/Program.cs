using ImGuiNET;
using SDL2;

namespace ReDalamud.Standalone;

public class Program
{
    private static ImGuiRenderer _renderer = null!;

    public static void Main(string[] args)
    {
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
            ImGui.ShowDemoWindow();
            _renderer.Render();
            SDL.SDL_GL_SwapWindow(_renderer.Window);
        }

        SDL.SDL_GL_DeleteContext(_renderer.GlContext);
        SDL.SDL_DestroyWindow(_renderer.Window);
        SDL.SDL_Quit();
    }
}