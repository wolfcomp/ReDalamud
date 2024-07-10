namespace ReDalamud.Standalone.Shaders;

public sealed class GLShader : IDisposable
{
    public uint ShaderId { get; private set; }
    public GL.ShaderType ShaderType { get; private set; }
    public string ShaderLog => GL.GetShaderInfoLog(ShaderId);

    #if DEBUG
    public string Source { get; private set; }
    #endif

    ~GLShader() => Dispose(false);

    public GLShader(GL.ShaderType shaderType, string source)
    {
        ShaderType = shaderType;
        ShaderId = GL.glCreateShader(shaderType);
        #if DEBUG
        Source = source;
        #endif

        GL.ShaderSource(ShaderId, source);
        GL.glCompileShader(ShaderId);

        if (!GL.GetShaderCompileStatus(ShaderId))
            throw new Exception($"Failed to compile shader: {ShaderLog}");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (ShaderId != 0)
        {
            GL.glDeleteShader(ShaderId);
            ShaderId = 0;
        }
    }
}