using Hexa.NET.ImGui.Backends.SDL3;
using ImSDLWindow = Hexa.NET.ImGui.Backends.SDL3.SDLWindow;
using SDLWindow = Hexa.NET.SDL3.SDLWindow;
using SDLGPUDevice = Hexa.NET.SDL3.SDLGPUDevice;
using ImSDLGPUDevice = Hexa.NET.ImGui.Backends.SDL3.SDLGPUDevice;

namespace ReDalamud.Standalone;

public partial class ImGuiRenderer
{
    public static unsafe void CreateWindow(ImUtf8String title, int width, int height, bool fullscreen = false)
    {
        const SDLInitFlags initFlags = SDLInitFlags.Events | SDLInitFlags.Video;
        SDL.Init((uint)initFlags);

        var windowFlags = SDLWindowFlags.Resizable | SDLWindowFlags.Hidden | SDLWindowFlags.HighPixelDensity;
        if (fullscreen)
            windowFlags |= SDLWindowFlags.Fullscreen;

        float mainScale = SDL.GetDisplayContentScale(SDL.GetPrimaryDisplay());
        SDLWindow* window = SDL.CreateWindow(title, (int)(width * mainScale), (int)(height * mainScale), (ulong)windowFlags);

        if (window == null)
        {
            throw new NullReferenceException($"Error: SDL_CreateWindow(): {SDL.GetErrorS()}");            
        }
        
        SDL.SetWindowPosition(window, (int)SDL.SDL_WINDOWPOS_CENTERED_MASK, (int)SDL.SDL_WINDOWPOS_CENTERED_MASK);
        SDL.ShowWindow(window);

        SDLGPUDevice* gpuDevice = SDL.CreateGPUDevice(
            (uint)(SDLGPUShaderFormat.Spirv | SDLGPUShaderFormat.Dxil | SDLGPUShaderFormat.Metallib),
            true, (byte*)null);

        if (gpuDevice == null)
        {
            throw new NullReferenceException($"Error: SDL_CreateGPUDevice(): {SDL.GetErrorS()}");
        }

        if (!SDL.ClaimWindowForGPUDevice(gpuDevice, window))
        {
            throw new NullReferenceException($"Error: SDL_ClaimWindowForGPUDevice(): {SDL.GetErrorS()}");
        }

        SDL.SetGPUSwapchainParameters(gpuDevice, window,
            SDLGPUSwapchainComposition.Sdr, SDLGPUPresentMode.Vsync);

        var ctx = ImGui.CreateContext();
        ImGui.SetCurrentContext(ctx);

        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        io.ConfigViewportsNoAutoMerge = false;
        io.ConfigViewportsNoTaskBarIcon = false;
#if DEBUG
        io.ConfigDebugIsDebuggerPresent = Debugger.IsAttached;
        io.ConfigErrorRecovery = true;
        io.ConfigErrorRecoveryEnableAssert = false;
        io.ConfigErrorRecoveryEnableDebugLog = false;
        io.ConfigErrorRecoveryEnableTooltip = true;
#endif
        
        ImGui.StyleColorsDark();
        var style = ImGui.GetStyle();
        style.ScaleAllSizes(mainScale);
        style.FontScaleDpi = mainScale;
        io.ConfigDpiScaleFonts = true;
        io.ConfigDpiScaleViewports = true;

        if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
        {
            style.WindowRounding = 0.0f;
            style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
        }

        
        ImGuiImplSDL3.SetCurrentContext(ctx);
        ImGuiImplSDL3.InitForSDLGPU((ImSDLWindow*)window);

        ImGuiImplSDLGPU3InitInfo initInfo = new()
        {
            Device = (ImSDLGPUDevice*)gpuDevice,
            ColorTargetFormat = (int)SDL.GetGPUSwapchainTextureFormat(gpuDevice, window),
            MSAASamples = (int)SDLGPUSampleCount.Samplecount1
        };

        ImGuiImplSDL3.SDLGPU3Init(ref initInfo);

        Instance = new ImGuiRenderer(window, gpuDevice, new(0.45f, 0.55f, 0.60f, 1.00f));
    }
}