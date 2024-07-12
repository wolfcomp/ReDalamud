using System.Runtime.InteropServices;
using ReDalamud.Standalone.Shaders;
using ReDalamud.Standalone.Utils;

namespace ReDalamud.Standalone;

public partial class ImGuiRenderer
{
    public readonly nint Window;
    public readonly nint GlContext;
    public GLShaderProgram ShaderProgram { get; private set; }
    private readonly uint _vboHandle;
    private readonly uint _elmHandle;
    private readonly uint _vaoHandle;
    private uint _fontTextureId;
    private float _time;
    private readonly bool[] _mousePressed = new bool[3];

    public ImGuiRenderer(nint window, nint glContext)
    {
        Window = window;
        GlContext = glContext;

        var (vertex, fragment) = Util.GetEmbeddedShaderFiles()["default"];

        ShaderProgram = new GLShaderProgram(vertex, fragment);

        ImGui.SetCurrentContext(ImGui.CreateContext());
        RebuildFontAtlas();
        InitKeyMap();
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        _vboHandle = GL.GenBuffer();
        _elmHandle = GL.GenBuffer();
        _vaoHandle = GL.GenVertexArray();
    }

    unsafe void RebuildFontAtlas()
    {
        var fonts = ImGui.GetIO().Fonts;
        fonts.AddFontDefault();
        fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int _);

        _fontTextureId = LoadTexture((nint)pixelData, width, height);

        fonts.TexID = (nint)_fontTextureId;
        fonts.ClearTexData();
    }

    public void Render()
    {
        PrepareGLContext();
        ImGui.Render();

        var io = ImGui.GetIO();
        GL.glViewport(0, 0, (int)io.DisplaySize.X, (int)io.DisplaySize.Y);
        GL.glClear(GL.ClearBufferMask.ColorBufferBit);

        RenderDrawData();
        GL.glDisable(GL.EnableCap.ScissorTest);
    }

    public void ClearColor(float r, float g, float b, float a)
    {
        GL.glClearColor(r, g, b, a);
    }

    void SetupRenderState(ImDrawDataPtr drawData, int fbWidth, int fbHeight)
    {
        GL.glEnable(GL.EnableCap.Blend);
        GL.glBlendEquation(GL.BlendEquationMode.FuncAdd);
        GL.glBlendFunc(GL.BlendingFactorSrc.SrcAlpha, GL.BlendingFactorDest.OneMinusSrcAlpha);
        GL.glDisable(GL.EnableCap.CullFace);
        GL.glDisable(GL.EnableCap.DepthTest);
        GL.glEnable(GL.EnableCap.ScissorTest);

        GL.glUseProgram(ShaderProgram.ProgramId);

        var left = drawData.DisplayPos.X;
        var right = drawData.DisplayPos.X + drawData.DisplaySize.X;
        var top = drawData.DisplayPos.Y;
        var bottom = drawData.DisplayPos.Y + drawData.DisplaySize.Y;

        ShaderProgram["Texture"].SetValue(0);
        ShaderProgram["ProjMtx"].SetValue(Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, -1, 1));
        GL.glBindSampler(0, 0);

        GL.glBindVertexArray(_vaoHandle);

        // Bind vertex/index buffers and setup attributes for ImDrawVert
        GL.glBindBuffer(GL.BufferTarget.ArrayBuffer, _vboHandle);
        GL.glBindBuffer(GL.BufferTarget.ElementArrayBuffer, _elmHandle);

        GL.EnableVertexAttribArray(ShaderProgram["Position"].Location);
        GL.EnableVertexAttribArray(ShaderProgram["UV"].Location);
        GL.EnableVertexAttribArray(ShaderProgram["Color"].Location);

        var drawVertSize = Marshal.SizeOf<ImDrawVert>();
        GL.VertexAttribPointer(ShaderProgram["Position"].Location, 2, GL.VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("pos"));
        GL.VertexAttribPointer(ShaderProgram["UV"].Location, 2, GL.VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("uv"));
        GL.VertexAttribPointer(ShaderProgram["Color"].Location, 4, GL.VertexAttribPointerType.UnsignedByte, true, drawVertSize, Marshal.OffsetOf<ImDrawVert>("col"));
    }

    unsafe void RenderDrawData()
    {
        var drawData = ImGui.GetDrawData();

        // Avoid rendering when minimized, scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
        var fbWidth = (int)(drawData.DisplaySize.X * drawData.FramebufferScale.X);
        var fbHeight = (int)(drawData.DisplaySize.Y * drawData.FramebufferScale.Y);
        if (fbWidth <= 0 || fbHeight <= 0)
            return;

        SetupRenderState(drawData, fbWidth, fbHeight);

        var clipOffset = drawData.DisplayPos;
        var clipScale = drawData.FramebufferScale;

        drawData.ScaleClipRects(clipScale);

        var lastTexId = ImGui.GetIO().Fonts.TexID;
        GL.glBindTexture(GL.TextureTarget.Texture2D, (uint)lastTexId);

        var drawVertSize = Marshal.SizeOf<ImDrawVert>();
        var drawIdxSize = sizeof(ushort);

        for (var n = 0; n < drawData.CmdListsCount; n++)
        {
            var cmdList = drawData.CmdLists[n];

            // Upload vertex/index buffers
            GL.glBufferData(GL.BufferTarget.ArrayBuffer, (IntPtr)(cmdList.VtxBuffer.Size * drawVertSize), cmdList.VtxBuffer.Data, GL.BufferUsageHint.StreamDraw);
            GL.glBufferData(GL.BufferTarget.ElementArrayBuffer, (IntPtr)(cmdList.IdxBuffer.Size * drawIdxSize), cmdList.IdxBuffer.Data, GL.BufferUsageHint.StreamDraw);

            for (var cmd_i = 0; cmd_i < cmdList.CmdBuffer.Size; cmd_i++)
            {
                var pcmd = cmdList.CmdBuffer[cmd_i];
                if (pcmd.UserCallback != IntPtr.Zero)
                {
                    Console.WriteLine("UserCallback not implemented");
                }
                else
                {

                    // Project scissor/clipping rectangles into framebuffer space
                    var clip_rect = pcmd.ClipRect;

                    clip_rect.X = pcmd.ClipRect.X - clipOffset.X;
                    clip_rect.Y = pcmd.ClipRect.Y - clipOffset.Y;
                    clip_rect.Z = pcmd.ClipRect.Z - clipOffset.X;
                    clip_rect.W = pcmd.ClipRect.W - clipOffset.Y;

                    GL.glScissor((int)clip_rect.X, (int)(fbHeight - clip_rect.W), (int)(clip_rect.Z - clip_rect.X), (int)(clip_rect.W - clip_rect.Y));

                    // Bind texture, Draw
                    if (pcmd.TextureId != IntPtr.Zero)
                    {
                        if (pcmd.TextureId != lastTexId)
                        {
                            lastTexId = pcmd.TextureId;
                            GL.glBindTexture(GL.TextureTarget.Texture2D, (uint)pcmd.TextureId);
                        }
                    }

                    GL.glDrawElementsBaseVertex(GL.BeginMode.Triangles, (int)pcmd.ElemCount, drawIdxSize == 2 ? GL.DrawElementsType.UnsignedShort : GL.DrawElementsType.UnsignedInt, (IntPtr)(pcmd.IdxOffset * drawIdxSize), (int)pcmd.VtxOffset);
                }
            }
        }
    }
    void InitKeyMap()
    {
        var io = ImGui.GetIO();

        io.KeyMap[(int)ImGuiKey.Tab] = (int)SDL_Scancode.SDL_SCANCODE_TAB;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)SDL_Scancode.SDL_SCANCODE_LEFT;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)SDL_Scancode.SDL_SCANCODE_RIGHT;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)SDL_Scancode.SDL_SCANCODE_UP;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)SDL_Scancode.SDL_SCANCODE_DOWN;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)SDL_Scancode.SDL_SCANCODE_PAGEUP;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)SDL_Scancode.SDL_SCANCODE_PAGEDOWN;
        io.KeyMap[(int)ImGuiKey.Home] = (int)SDL_Scancode.SDL_SCANCODE_HOME;
        io.KeyMap[(int)ImGuiKey.End] = (int)SDL_Scancode.SDL_SCANCODE_END;
        io.KeyMap[(int)ImGuiKey.Insert] = (int)SDL_Scancode.SDL_SCANCODE_INSERT;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)SDL_Scancode.SDL_SCANCODE_DELETE;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)SDL_Scancode.SDL_SCANCODE_BACKSPACE;
        io.KeyMap[(int)ImGuiKey.Space] = (int)SDL_Scancode.SDL_SCANCODE_SPACE;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)SDL_Scancode.SDL_SCANCODE_RETURN;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)SDL_Scancode.SDL_SCANCODE_ESCAPE;
        io.KeyMap[(int)ImGuiKey.KeypadEnter] = (int)SDL_Scancode.SDL_SCANCODE_RETURN2;
        io.KeyMap[(int)ImGuiKey.A] = (int)SDL_Scancode.SDL_SCANCODE_A;
        io.KeyMap[(int)ImGuiKey.C] = (int)SDL_Scancode.SDL_SCANCODE_C;
        io.KeyMap[(int)ImGuiKey.V] = (int)SDL_Scancode.SDL_SCANCODE_V;
        io.KeyMap[(int)ImGuiKey.X] = (int)SDL_Scancode.SDL_SCANCODE_X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)SDL_Scancode.SDL_SCANCODE_Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)SDL_Scancode.SDL_SCANCODE_Z;
    }

    public void NewFrame()
    {
        ImGui.NewFrame();
        var io = ImGui.GetIO();

        // Setup display size (every frame to accommodate for window resizing)
        SDL_GetWindowSize(Window, out var w, out var h);
        SDL_GL_GetDrawableSize(Window, out var displayW, out var displayH);
        io.DisplaySize = new Vector2(w, h);
        if (w > 0 && h > 0)
            io.DisplayFramebufferScale = new Vector2((float)displayW / w, (float)displayH / h);

        // Setup time step (we don't use SDL_GetTicks() because it is using millisecond resolution)
        var frequency = SDL_GetPerformanceFrequency();
        var currentTime = SDL_GetPerformanceCounter();
        io.DeltaTime = _time > 0 ? (float)((double)(currentTime - _time) / frequency) : 1.0f / 60.0f;
        if (io.DeltaTime <= 0)
            io.DeltaTime = 0.016f;
        _time = currentTime;

        UpdateMousePosAndButtons();
    }

    public unsafe void ProcessEvent(SDL_Event evt)
    {
        var io = ImGui.GetIO();
        switch (evt.type)
        {
            case SDL_EventType.SDL_MOUSEWHEEL:
                {
                    if (evt.wheel.x > 0) io.MouseWheelH += 1;
                    if (evt.wheel.x < 0) io.MouseWheelH -= 1;
                    if (evt.wheel.y > 0) io.MouseWheel += 1;
                    if (evt.wheel.y < 0) io.MouseWheel -= 1;
                    return;
                }
            case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                {
                    if (evt.button.button == SDL_BUTTON_LEFT) _mousePressed[0] = true;
                    if (evt.button.button == SDL_BUTTON_RIGHT) _mousePressed[1] = true;
                    if (evt.button.button == SDL_BUTTON_MIDDLE) _mousePressed[2] = true;
                    return;
                }
            case SDL_EventType.SDL_TEXTINPUT:
                {
                    var str = new string((sbyte*)evt.text.text);
                    io.AddInputCharactersUTF8(str);
                    return;
                }
            case SDL_EventType.SDL_KEYDOWN:
            case SDL_EventType.SDL_KEYUP:
                {
                    var key = evt.key.keysym.scancode;
                    io.KeysDown[(int)key] = evt.type == SDL_EventType.SDL_KEYDOWN;
                    io.KeyShift = (SDL_GetModState() & SDL_Keymod.KMOD_SHIFT) != 0;
                    io.KeyCtrl = (SDL_GetModState() & SDL_Keymod.KMOD_CTRL) != 0;
                    io.KeyAlt = (SDL_GetModState() & SDL_Keymod.KMOD_ALT) != 0;
                    io.KeySuper = (SDL_GetModState() & SDL_Keymod.KMOD_GUI) != 0;
                    break;
                }
        }
    }

    void UpdateMousePosAndButtons()
    {
        var io = ImGui.GetIO();

        // Set OS mouse position if requested (rarely used, only when ImGuiConfigFlags_NavEnableSetMousePos is enabled by user)
        if (io.WantSetMousePos)
            SDL_WarpMouseInWindow(Window, (int)io.MousePos.X, (int)io.MousePos.Y);
        else
            io.MousePos = new Vector2(float.MinValue, float.MinValue);

        var mouseButtons = SDL_GetMouseState(out var mx, out var my);
        io.MouseDown[0] =
            _mousePressed[0] ||
            (mouseButtons & SDL_BUTTON(SDL_BUTTON_LEFT)) !=
            0; // If a mouse press event came, always pass it as "mouse held this frame", so we don't miss click-release events that are shorter than 1 frame.
        io.MouseDown[1] = _mousePressed[1] || (mouseButtons & SDL_BUTTON(SDL_BUTTON_RIGHT)) != 0;
        io.MouseDown[2] = _mousePressed[2] || (mouseButtons & SDL_BUTTON(SDL_BUTTON_MIDDLE)) != 0;
        _mousePressed[0] = _mousePressed[1] = _mousePressed[2] = false;

        var focusedWindow = SDL_GetKeyboardFocus();
        if (Window == focusedWindow)
        {
            // SDL_GetMouseState() gives mouse position seemingly based on the last window entered/focused(?)
            // The creation of a new windows at runtime and SDL_CaptureMouse both seems to severely mess up with that, so we retrieve that position globally.
            SDL_GetWindowPosition(focusedWindow, out var wx, out var wy);
            SDL_GetGlobalMouseState(out mx, out my);
            mx -= wx;
            my -= wy;
            io.MousePos = new Vector2(mx, my);
        }

        // SDL_CaptureMouse() let the OS know e.g. that our imgui drag outside the SDL window boundaries shouldn't e.g. trigger the OS window resize cursor.
        var any_mouse_button_down = ImGui.IsAnyMouseDown();
        SDL_CaptureMouse(any_mouse_button_down ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
    }

    void PrepareGLContext() => SDL_GL_MakeCurrent(Window, GlContext);

    public void Dispose()
    {
        if (ShaderProgram != null)
        {
            ShaderProgram.Dispose();
            ShaderProgram = null;
            GL.DeleteBuffer(_vboHandle);
            GL.DeleteBuffer(_elmHandle);
            GL.DeleteVertexArray(_vaoHandle);
            GL.DeleteTexture(_fontTextureId);
        }
    }
}