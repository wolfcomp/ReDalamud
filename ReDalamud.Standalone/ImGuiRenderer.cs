using Hexa.NET.ImGui.Backends.SDL3;
using ImSDLEvent = Hexa.NET.ImGui.Backends.SDL3.SDLEvent;
using SDLWindow = Hexa.NET.SDL3.SDLWindow;
using SDLEvent = Hexa.NET.SDL3.SDLEvent;
using SDLGPUDevice = Hexa.NET.SDL3.SDLGPUDevice;

namespace ReDalamud.Standalone;

public unsafe partial class ImGuiRenderer
{
    public static ImGuiRenderer Instance;

    private ImTextureID _fontTextureId;
    private float _time;
    private SDLWindow* _window;
    private SDLGPUDevice* _gpuDevice;
    private Vector4 _clearColor;

    public ImGuiRenderer(SDLWindow* window, SDLGPUDevice* gpuDevice, Vector4 clearColor)
    {
        _window = window;
        _gpuDevice = gpuDevice;
        _clearColor = clearColor;
    }

    public bool ShouldRender() => ((SDLWindowFlags)SDL.GetWindowFlags(_window) & SDLWindowFlags.Minimized) != 0;

    public bool ProcessExit()
    {
        SDLEvent e = default;
        while (SDL.PollEvent(ref e))
        {
            ImGuiImplSDL3.ProcessEvent((ImSDLEvent*)&e);
            var type = (SDLEventType)e.Type;
            if (type == SDLEventType.Quit || (type == SDLEventType.WindowCloseRequested &&
                                              e.Window.WindowID == SDL.GetWindowID(_window)))
                return true;
        }

        return false;
    }

    public void Dispose()
    {
        SDL.WaitForGPUIdle(_gpuDevice);
        ImGuiImplSDL3.Shutdown();
        ImGuiImplSDL3.SDLGPU3Shutdown();
        ImGui.DestroyContext();

        IconLoader.Dispose();

        SDL.ReleaseWindowFromGPUDevice(_gpuDevice, _window);
        SDL.DestroyGPUDevice(_gpuDevice);
        SDL.DestroyWindow(_window);
    }
}