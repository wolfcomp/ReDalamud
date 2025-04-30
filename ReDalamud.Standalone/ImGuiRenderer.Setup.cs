namespace ReDalamud.Standalone;

public partial class ImGuiRenderer
{
    public static unsafe ImGuiRenderer CreateWindowAndGlContext(string title, int width, int height, bool fullscreen = false,
        bool highDpi = false)
    {
        SDL.SetHint(SDL.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
        SDL.Init(SDLInitFlags.Events | SDLInitFlags.Video);

        var windowFlags = SDLWindowFlags.Opengl | SDLWindowFlags.Resizable;
        if (fullscreen)
            windowFlags |= SDLWindowFlags.Fullscreen;
        if (highDpi)
            windowFlags |= SDLWindowFlags.AllowHighdpi;

        var window = SDL.CreateWindow(title, width, height, windowFlags);

        var guiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(guiContext);

        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        io.ConfigViewportsNoAutoMerge = false;
        io.ConfigViewportsNoTaskBarIcon = false;
            
        var glContext = SDL.GLCreateContext(window);

        ImGuiImplSDL3.SetCurrentContext(guiContext);
        if (!ImGuiImplSDL3.SDL3InitForOpenGL(new SDLWindowPtr((Hexa.NET.ImGui.Backends.SDL3.SDLWindow*)window),
                (void*)glContext.Handle))
        {
            SDL.Quit();
            throw new Exception("ImGuiImplSDL3.SDL3InitForOpenGL failed");
        }
        ImGuiImplOpenGL3.SetCurrentContext(guiContext);
        if (!ImGuiImplOpenGL3.Init((byte*)null))
        {
            SDL.Quit();
            throw new Exception("ImGuiImplOpenGL3.Init failed");
        }

        return new ImGuiRenderer(window, glContext);
    }

    public static ImTextureID LoadTexture(nint pixelData, int width, int height, GLPixelFormat format = GLPixelFormat.Rgba, GLInternalFormat internalFormat = GLInternalFormat.Rgba)
    {
        var textureId = GL.GenTexture();
        GL.PixelStorei(GLPixelStoreParameter.UnpackAlignment, 1);
        GL.BindTexture(GLTextureTarget.Texture2D, textureId);
        GL.TexImage2D(GLTextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, GLPixelType.UnsignedByte, pixelData);
        GL.TexParameteri(GLTextureTarget.Texture2D, GLTextureParameterName.MagFilter, (int)GLTextureMagFilter.Linear);
        GL.TexParameteri(GLTextureTarget.Texture2D, GLTextureParameterName.MinFilter, (int)GLTextureMinFilter.Linear);
        GL.BindTexture(GLTextureTarget.Texture2D, 0);
        return textureId;
    }
}