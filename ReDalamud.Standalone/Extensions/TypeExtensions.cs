namespace ReDalamud.Standalone.Extensions;
public static class TypeExtensions
{
    public static string GetDocString(this Type type) => type switch
    {
        _ when type == typeof(Vector4) => "# Vector4: X,Y,Z,W",
        _ when type == typeof(Color) => "# Color: R,G,B,A",
        _ => string.Empty
    };
}
