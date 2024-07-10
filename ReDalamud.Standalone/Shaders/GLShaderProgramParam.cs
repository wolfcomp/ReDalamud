using System.Numerics;

namespace ReDalamud.Standalone.Shaders;

public enum ParamType
{
    Uniform,
    Attribute
}

public sealed class GLShaderProgramParam(Type type, ParamType paramType, string name)
{
    public Type Type { get; private set; } = type;

    public int Location { get; private set; }

    public uint ProgramId { get; private set; }
    
    public ParamType ParamType { get; private set; } = paramType;

    public string Name { get; private set; } = name;

    public GLShaderProgramParam(Type type, ParamType paramType, string name, uint programId, int location) : this(type, paramType, name)
    {
        ProgramId = programId;
        Location = location;
    }

    public void GetLocation(GLShaderProgram program)
    {
        program.Use();
        if (ProgramId == 0)
        {
            ProgramId = program.ProgramId;
            Location = ParamType == ParamType.Uniform ? program.GetUniformLocation(Name) : program.GetAttributeLocation(Name);
        }
    }

    public void SetValue(bool value) => GL.glUniform1i(Location, value ? 1 : 0);

    public void SetValue(int value) => GL.glUniform1i(Location, value);

    public void SetValue(float value) => GL.glUniform1f(Location, value);

    public void SetValue(Vector2 value) => GL.glUniform2f(Location, value.X, value.Y);

    public void SetValue(Vector3 value) => GL.glUniform3f(Location, value.X, value.Y, value.Z);

    public void SetValue(Vector4 value) => GL.glUniform4f(Location, value.X, value.Y, value.Z, value.W);

    public void SetValue(Matrix4x4 value) => GL.UniformMatrix4fv(Location, value);

    public void SetValue(float[] value)
    {
        switch (value.Length)
        {
            case 16:
                GL.glUniformMatrix4fv(Location, 1, false, value);
                break;
            case 9:
                GL.glUniformMatrix3fv(Location, 1, false, value);
                break;
            default:
                throw new Exception("Invalid matrix size");
        }
    }
}