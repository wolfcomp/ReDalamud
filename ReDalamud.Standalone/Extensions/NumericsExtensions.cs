namespace ReDalamud.Standalone.Extensions;
public static class NumericsExtensions
{
    public static string ToIniString(this System.Numerics.Vector4 vector)
    {
        return $"{vector.X},{vector.Y},{vector.Z},{vector.W}";
    }

    public static System.Numerics.Vector4 FromIniString(string str)
    {
        var split = str.Split(',');
        return new System.Numerics.Vector4(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
    }
}
