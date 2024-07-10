using static SDL2.SDL;
using static ReDalamud.Standalone.GL;

namespace ReDalamud.Standalone;

public partial class ImGuiRenderer
{
    public static ImGuiRenderer CreateWindowAndGlContext(string title, int width, int height, bool fullscreen = false,
        bool highDpi = false)
    {
        SDL_Init(SDL_INIT_VIDEO);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_FLAGS, (int)SDL_GLcontext.SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, (int)SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);

        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
        SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_STENCIL_SIZE, 8);

        var windowFlags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
        if (fullscreen)
            windowFlags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
        if (highDpi)
            windowFlags |= SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;

        var window = SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, windowFlags);
        var glContext = CreateGlContext(window);
        return new ImGuiRenderer(window, glContext);
    }

    static nint CreateGlContext(nint window)
    {
        var glContext = SDL_GL_CreateContext(window);
        if (glContext == nint.Zero)
            throw new Exception("Failed to create OpenGL context.");

        SDL_GL_MakeCurrent(window, glContext);
        SDL_GL_SetSwapInterval(1);

        glClearColor(0f, 0f, 0f, 1f);
        glClear(ClearBufferMask.ColorBufferBit);
        SDL_GL_SwapWindow(window);

        Console.WriteLine($"GL Version: {glGetString(StringName.Version)}");
        return glContext;
    }

    public static uint LoadTexture(nint pixelData, int width, int height, PixelFormat format = PixelFormat.Rgba, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba)
    {
        var textureId = GenTexture();
        glPixelStorei(PixelStoreParameter.UnpackAlignment, 1);
        glBindTexture(TextureTarget.Texture2D, textureId);
        glTexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, PixelType.UnsignedByte, pixelData);
        glTexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
        glTexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
        glBindTexture(TextureTarget.Texture2D, 0);
        return textureId;
    }
}