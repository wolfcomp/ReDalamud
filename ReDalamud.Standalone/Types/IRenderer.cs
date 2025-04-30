namespace ReDalamud.Standalone.Types;
public interface IRenderer
{
    bool HasName { get; }
    bool HasCode { get; }
    int Size { get; }
    void DrawMemory(nint address, int offset);
    void DrawCSharpCode();
    float GetHeight();
}

public interface IUnknownRenderer : IRenderer
{
    public void DrawToolTip();
}