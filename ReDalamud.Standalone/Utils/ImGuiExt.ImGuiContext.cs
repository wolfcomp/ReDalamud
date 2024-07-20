using System.Runtime.InteropServices;

namespace ReDalamud.Standalone.Utils;
public static partial class ImGuiExt
{

}

[StructLayout(LayoutKind.Explicit)]
public struct ImGuiContext
{
    [FieldOffset(0x0)] public byte Initialized;
    [FieldOffset(0x1)] public byte FontAtlasOwnedByContext;
    [FieldOffset(0x8)] public ImGuiIO IO;
    [FieldOffset(0x37B8)] public ImGuiPlatformIO PlatformIO;
    [FieldOffset(0x382C)] public ImGuiStyle Style;
}