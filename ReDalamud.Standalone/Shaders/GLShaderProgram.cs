using System.Text;

namespace ReDalamud.Standalone.Shaders;

public sealed class GLShaderProgram : IDisposable
{
    public uint ProgramId { get; private set; }
    public GLShader VertexShader { get; private set; }
    public GLShader FragmentShader { get; private set; }

    private Dictionary<string, GLShaderProgramParam> shaderParams;

    public GLShaderProgramParam this[string name] => shaderParams.TryGetValue(name, out var value) ? value : null;

    public string ProgramLog => GL.GetProgramInfoLog(ProgramId);

    public GLShaderProgram(GLShader vertexShader, GLShader fragmentShader)
    {
        VertexShader = vertexShader;
        FragmentShader = fragmentShader;
        ProgramId = GL.glCreateProgram();
        GL.glAttachShader(ProgramId, vertexShader.ShaderId);
        GL.glAttachShader(ProgramId, fragmentShader.ShaderId);
        GL.glLinkProgram(ProgramId);

        if (!GL.GetProgramLinkStatus(ProgramId))
            throw new Exception($"Failed to link shader program: {ProgramLog}");

        GetParams();
    }

    public GLShaderProgram(string vertexShaderSource, string fragmentShaderSource)
        : this(new GLShader(GL.ShaderType.VertexShader, vertexShaderSource), new GLShader(GL.ShaderType.FragmentShader, fragmentShaderSource))
    {
    }

    void GetParams()
    {
        shaderParams = new Dictionary<string, GLShaderProgramParam>();

        var resources = new int[1];
        var actualLength = new int[1];
        var arraySize = new int[1];

        GL.glGetProgramiv(ProgramId, GL.ProgramParameter.ActiveAttributes, resources);
        for (uint i = 0; i < resources[0]; i++)
        {
            var type = new GL.ActiveAttribType[1];
            var sb = new StringBuilder(256);
            GL.glGetActiveAttrib(ProgramId, i, sb.Capacity, actualLength, arraySize, type, sb);
            var name = sb.ToString();
            if (!shaderParams.ContainsKey(name))
            {
                var param = new GLShaderProgramParam(TypeFromAttributeType(type[0]), ParamType.Attribute, name);
                shaderParams.Add(name, param);
                param.GetLocation(this);
            }
        }

        GL.glGetProgramiv(ProgramId, GL.ProgramParameter.ActiveUniforms, resources);
        for (uint i = 0; i < resources[0]; i++)
        {
            var type = new GL.ActiveUniformType[1];
            var sb = new StringBuilder(256);
            GL.glGetActiveUniform(ProgramId, i, sb.Capacity, actualLength, arraySize, type, sb);
            var name = sb.ToString();
            if (!shaderParams.ContainsKey(name))
            {
                var param = new GLShaderProgramParam(TypeFromUniformType(type[0]), ParamType.Uniform, name);
                shaderParams.Add(name, param);
                param.GetLocation(this);
            }
        }
    }

    Type TypeFromAttributeType(GL.ActiveAttribType type) => type switch
    {
        GL.ActiveAttribType.Float => typeof(float),
        GL.ActiveAttribType.FloatVec2 => typeof(Vector2),
        GL.ActiveAttribType.FloatVec3 => typeof(Vector3),
        GL.ActiveAttribType.FloatVec4 => typeof(Vector4),
        GL.ActiveAttribType.FloatMat2 => typeof(float[]),
        GL.ActiveAttribType.FloatMat3 => typeof(float[]),
        GL.ActiveAttribType.FloatMat4 => typeof(Matrix4x4),
        _ => typeof(object)
    };

    Type TypeFromUniformType(GL.ActiveUniformType type) => type switch
    {
        GL.ActiveUniformType.Int => typeof(int),
        GL.ActiveUniformType.Float => typeof(float),
        GL.ActiveUniformType.FloatVec2 => typeof(Vector2),
        GL.ActiveUniformType.FloatVec3 => typeof(Vector3),
        GL.ActiveUniformType.FloatVec4 => typeof(Vector4),
        GL.ActiveUniformType.IntVec2 => typeof(int[]),
        GL.ActiveUniformType.IntVec3 => typeof(int[]),
        GL.ActiveUniformType.IntVec4 => typeof(int[]),
        GL.ActiveUniformType.Bool => typeof(bool),
        GL.ActiveUniformType.BoolVec2 => typeof(bool[]),
        GL.ActiveUniformType.BoolVec3 => typeof(bool[]),
        GL.ActiveUniformType.BoolVec4 => typeof(bool[]),
        GL.ActiveUniformType.FloatMat2 => typeof(float[]),
        GL.ActiveUniformType.FloatMat3 => typeof(float[]),
        GL.ActiveUniformType.FloatMat4 => typeof(float[]),
        GL.ActiveUniformType.Sampler1D => typeof(int),
        GL.ActiveUniformType.Sampler2D => typeof(int),
        GL.ActiveUniformType.Sampler3D => typeof(int),
        GL.ActiveUniformType.SamplerCube => typeof(int),
        GL.ActiveUniformType.Sampler1DShadow => typeof(int),
        GL.ActiveUniformType.Sampler2DShadow => typeof(int),
        GL.ActiveUniformType.Sampler2DRect => typeof(int),
        GL.ActiveUniformType.Sampler2DRectShadow => typeof(int),
        GL.ActiveUniformType.FloatMat2x3 => typeof(float[]),
        GL.ActiveUniformType.FloatMat2x4 => typeof(float[]),
        GL.ActiveUniformType.FloatMat3x2 => typeof(float[]),
        GL.ActiveUniformType.FloatMat3x4 => typeof(float[]),
        GL.ActiveUniformType.FloatMat4x2 => typeof(float[]),
        GL.ActiveUniformType.FloatMat4x3 => typeof(float[]),
        GL.ActiveUniformType.Sampler1DArray => typeof(int),
        GL.ActiveUniformType.Sampler2DArray => typeof(int),
        GL.ActiveUniformType.SamplerBuffer => typeof(int),
        GL.ActiveUniformType.Sampler1DArrayShadow => typeof(int),
        GL.ActiveUniformType.Sampler2DArrayShadow => typeof(int),
        GL.ActiveUniformType.SamplerCubeShadow => typeof(int),
        GL.ActiveUniformType.UnsignedIntVec2 => typeof(uint[]),
        GL.ActiveUniformType.UnsignedIntVec3 => typeof(uint[]),
        GL.ActiveUniformType.UnsignedIntVec4 => typeof(uint[]),
        GL.ActiveUniformType.IntSampler1D => typeof(int),
        GL.ActiveUniformType.IntSampler2D => typeof(int),
        GL.ActiveUniformType.IntSampler3D => typeof(int),
        GL.ActiveUniformType.IntSamplerCube => typeof(int),
        GL.ActiveUniformType.IntSampler2DRect => typeof(int),
        GL.ActiveUniformType.IntSampler1DArray => typeof(int),
        GL.ActiveUniformType.IntSampler2DArray => typeof(int),
        GL.ActiveUniformType.IntSamplerBuffer => typeof(int),
        GL.ActiveUniformType.UnsignedIntSampler1D => typeof(uint),
        GL.ActiveUniformType.UnsignedIntSampler2D => typeof(uint),
        GL.ActiveUniformType.UnsignedIntSampler3D => typeof(uint),
        GL.ActiveUniformType.UnsignedIntSamplerCube => typeof(uint),
        GL.ActiveUniformType.UnsignedIntSampler2DRect => typeof(uint),
        GL.ActiveUniformType.UnsignedIntSampler1DArray => typeof(uint),
        GL.ActiveUniformType.UnsignedIntSampler2DArray => typeof(uint),
        GL.ActiveUniformType.UnsignedIntSamplerBuffer => typeof(uint),
        GL.ActiveUniformType.Sampler2DMultisample => typeof(int),
        GL.ActiveUniformType.IntSampler2DMultisample => typeof(int),
        GL.ActiveUniformType.UnsignedIntSampler2DMultisample => typeof(uint),
        GL.ActiveUniformType.Sampler2DMultisampleArray => typeof(int),
        GL.ActiveUniformType.IntSampler2DMultisampleArray => typeof(int),
        GL.ActiveUniformType.UnsignedIntSampler2DMultisampleArray => typeof(uint),
        _ => typeof(object)
    };

    public void Use() => GL.glUseProgram(ProgramId);

    public int GetUniformLocation(string name)
    {
        Use();
        return GL.glGetUniformLocation(ProgramId, name);
    }

    public int GetAttributeLocation(string name)
    {
        Use();
        return GL.glGetAttribLocation(ProgramId, name);
    }

    ~GLShaderProgram() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (ProgramId != 0)
        {
            GL.glUseProgram(0);
            GL.glDetachShader(ProgramId, VertexShader.ShaderId);
            GL.glDetachShader(ProgramId, FragmentShader.ShaderId);
            GL.glDeleteProgram(ProgramId);
            ProgramId = 0;
            VertexShader.Dispose();
            FragmentShader.Dispose();
        }
    }
}