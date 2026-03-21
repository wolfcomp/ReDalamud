using System.Runtime.CompilerServices;
// ReSharper disable MemberCanBePrivate.Global

namespace ReDalamud.Standalone.Extensions;
public static unsafe class NumericsExtensions
{
    public static string ToIniString(this Vector4 vector)
    {
        return string.Join(',', new ReadOnlySpan<float>(Unsafe.AsPointer(ref vector), 4).ToArray().Select(ToIniString));
    }

    public static Vector4 FromIniStringVector4(string str)
    {
        return new Vector4(str.Split(',').Select(FromIniStringFloat).ToArray());
    }

    public static string ToIniString(this Vector3 vector)
    {
        return string.Join(',', new ReadOnlySpan<float>(Unsafe.AsPointer(ref vector), 3).ToArray().Select(ToIniString));
    }

    public static Vector3 FromIniStringVector3(string str)
    {
        return new Vector3(str.Split(',').Select(FromIniStringFloat).ToArray());
    }

    public static string ToIniString(this Vector2 vector)
    {
        return string.Join(',', new ReadOnlySpan<float>(Unsafe.AsPointer(ref vector), 2).ToArray().Select(ToIniString));
    }

    public static Vector2 FromIniStringVector2(string str)
    {
        return new Vector2(str.Split(',').Select(FromIniStringFloat).ToArray());
    }

    public static string ToIniString(this float flt)
    {
        return Unsafe.As<float, uint>(ref flt).ToString("X8");
    }

    public static float FromIniStringFloat(string str)
    {
        var num = uint.Parse(str, System.Globalization.NumberStyles.HexNumber);
        return Unsafe.As<uint, float>(ref num);
    }
}
