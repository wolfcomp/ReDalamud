namespace ReDalamud.Standalone.Types;
public interface IRenderer
{
    bool HasCode { get; }
    int Size { get; }
    void DrawMemory(nint address);
    void DrawCSharpCode();
    float GetHeight();
}
