using SDLWindow = Hexa.NET.SDL3.SDLWindow;

namespace ReDalamud.Standalone;

public partial class ImGuiRenderer
{
    public unsafe class BindingContext : HexaGen.Runtime.IGLContext
    {
        private readonly SDLWindow* _window;
        private readonly SDLGLContext _glContext;

        public BindingContext(SDLWindow* window, SDLGLContext glContext)
        {
            _window = window;
            _glContext = glContext;
        }

        public nint Handle => (nint)_window;

        public bool IsCurrent => SDL.GLGetCurrentContext() == _glContext;

        public void Dispose()
        {

        }

        public nint GetProcAddress(string procName) => (nint)SDL.GLGetProcAddress(procName);

        public bool IsExtensionSupported(string extension) => SDL.GLExtensionSupported(extension);

        public void MakeCurrent() => SDL.GLMakeCurrent(_window, _glContext);

        public void SwapBuffers() => SDL.GLSwapWindow(_window);

        public void SwapInterval(int interval) => SDL.GLSetSwapInterval(interval);

        public bool TryGetProcAddress(string procName, out nint address)
        {
            address = GetProcAddress(procName);
            return address != 0;
        }
    }
}