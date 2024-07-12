using System.Globalization;

namespace ReDalamud.Standalone;
public class Color
{
    public Vector4 InternalValue;

    public float R
    {
        get => InternalValue.X;
        set => InternalValue.X = value;
    }

    public float G
    {
        get => InternalValue.Y;
        set => InternalValue.Y = value;
    }

    public float B
    {
        get => InternalValue.Z;
        set => InternalValue.Z = value;
    }

    public float A
    {
        get => InternalValue.W;
        set => InternalValue.W = value;
    }

    public static Color FromRGBAInt(int r, int g, int b, int a) => FromRGBAFloat(r / 255f, g / 255f, b / 255f, a / 255f);
    public static Color FromRGBInt(int r, int g, int b) => FromRGBFloat(r / 255f, g / 255f, b / 255f);
    public static Color FromRGBAFloat(float r, float g, float b, float a) => new() { InternalValue = new Vector4(r, g, b, a) };
    public static Color FromRGBFloat(float r, float g, float b) => FromRGBAFloat(r,g,b,1);

    public static Color FromHex(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex[1..];
        return hex.Length switch
        {
            6 => FromRGBInt(int.Parse(hex[..2], NumberStyles.HexNumber), int.Parse(hex[2..4], NumberStyles.HexNumber),
                int.Parse(hex[4..6], NumberStyles.HexNumber)),
            8 => FromRGBAInt(int.Parse(hex[..2], NumberStyles.HexNumber), int.Parse(hex[2..4], NumberStyles.HexNumber),
                int.Parse(hex[4..6], NumberStyles.HexNumber), int.Parse(hex[6..8], NumberStyles.HexNumber)),
            _ => throw new ArgumentException("Hex string must be 6 or 8 characters long.")
        };
    }

    public string ToHex(bool includeAlpha) => $"#{(uint)(R * 255):X2}{(uint)(G * 255):X2}{(uint)(B * 255):X2}{(includeAlpha ? $"{(uint)(A * 255):X2}": string.Empty)}";

    public override string ToString() => ToHex(true);

    public static implicit operator Vector4(Color color) => color.InternalValue;
    public static implicit operator Color(Vector4 vector) => new() { InternalValue = vector };
    public static implicit operator uint(Color color) => (uint)(color.InternalValue.X * 255) << 24 | (uint)(color.InternalValue.Y * 255) << 16 | (uint)(color.InternalValue.Z * 255) << 8 | (uint)(color.InternalValue.W * 255);
    public static implicit operator Color(uint color) => FromRGBAFloat((color >> 24 & 0xFF) / 255f, (color >> 16 & 0xFF) / 255f, (color >> 8 & 0xFF) / 255f, (color & 0xFF) / 255f);
    public static implicit operator string(Color color) => color.ToHex(true);
    public static implicit operator Color(string color) => FromHex(color);
}
