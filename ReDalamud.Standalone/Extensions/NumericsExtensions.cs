namespace ReDalamud.Standalone.Extensions;
public static class NumericsExtensions
{
    public static string ToIniString(this Vector4 vector)
    {
        return $"{vector.X},{vector.Y},{vector.Z},{vector.W}";
    }

    public static Vector4 FromIniStringVector4(string str)
    {
        var split = str.Split(',');
        return new Vector4(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
    }
    public static string ToIniString(this Vector2 vector)
    {
        return $"{vector.X},{vector.Y}";
    }

    public static Vector2 FromIniStringVector2(string str)
    {
        var split = str.Split(',');
        return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
    }
}
