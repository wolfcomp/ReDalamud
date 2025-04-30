using SDLWindow = Hexa.NET.SDL3.SDLWindow;

namespace ReDalamud.Standalone;

public unsafe partial class ImGuiRenderer
{
    public static GL GL { get; private set; }
    private ImTextureID _fontTextureId;
    private float _time;
    private SDLWindow* _window;
    public uint WindowId => _window != null ? SDL.GetWindowID(_window) : 0;

    public ImGuiRenderer(SDLWindow* window, SDLGLContext glContext)
    {
        GL = new GL(new BindingContext(window, glContext));
        // RebuildFontAtlas();
    }

    // unsafe void RebuildFontAtlas()
    // {
    //     var fonts = ImGui.GetIO().Fonts;
    //     fonts.AddFontDefault();
    //     fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int _);
    //
    //     _fontTextureId = LoadTexture((nint)pixelData, width, height);
    //
    //     fonts.TexID = _fontTextureId;
    //     fonts.ClearTexData();
    // }

    public void Render()
    {
        ImGui.Render();
        ImGui.EndFrame();

        GL.MakeCurrent();
        ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

        if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
        {
            ImGui.UpdatePlatformWindows();
            ImGui.RenderPlatformWindowsDefault();
        }

        GL.MakeCurrent();

        GL.SwapBuffers();
    }

    public void Clear(float r, float g, float b, float a)
    {
        GL.MakeCurrent();
        GL.ClearColor(r, g, b, a);
        GL.Clear(GLClearBufferMask.ColorBufferBit);
    }

    public void NewFrame()
    {
        ImGuiImplOpenGL3.NewFrame();
        ImGuiImplSDL3.SDL3NewFrame();
        ImGui.NewFrame();
    }

    public void Dispose()
    {
        GL.Dispose();
        SDL.DestroyWindow(_window);
    }
}