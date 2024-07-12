namespace ReDalamud.Standalone.Utils;
public static class ConvertBits
{
    public static byte[] GetBytes(bool value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, bool value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(char value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, char value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(short value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, short value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, int value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, long value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(ushort value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, ushort value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(Half value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, Half value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(float value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, float value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static byte[] GetBytes(double value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    public static bool TryWriteBytes(Span<byte> destination, double value)
    {
        var bytes = BitConverter.GetBytes(value);
        bytes.CopyTo(destination);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            destination.Reverse();
        return true;
    }

    public static char ToChar(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToChar(value, startIndex);
    }

    public static char ToChar(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToChar(value);
    }

    public static short ToInt16(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToInt16(value, startIndex);
    }

    public static short ToInt16(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToInt16(value);
    }

    public static int ToInt32(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToInt32(value, startIndex);
    }

    public static int ToInt32(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToInt32(value);
    }

    public static long ToInt64(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToInt64(value, startIndex);
    }

    public static long ToInt64(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToInt64(value);
    }

    public static ushort ToUInt16(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToUInt16(value, startIndex);
    }

    public static ushort ToUInt16(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToUInt16(value);
    }

    public static uint ToUInt32(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToUInt32(value, startIndex);
    }

    public static uint ToUInt32(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToUInt32(value);
    }

    public static ulong ToUInt64(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToUInt64(value, startIndex);
    }

    public static ulong ToUInt64(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToUInt64(value);
    }

    public static Half ToHalf(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToHalf(value, startIndex);
    }

    public static Half ToHalf(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToHalf(value);
    }

    public static float ToSingle(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToSingle(value, startIndex);
    }

    public static float ToSingle(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToSingle(value);
    }

    public static double ToDouble(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToDouble(value, startIndex);
    }

    public static double ToDouble(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToDouble(value);
    }

    public static string ToString(byte[] value, int startIndex, int length)
    {
        var arrCopy = value[startIndex..(startIndex + length)];
        if (Config.Global.IsLittleEndian)
            Array.Reverse(arrCopy);
        return BitConverter.ToString(arrCopy);
    }

    public static string ToString(byte[] value)
    {
        var arrCopy = value[..];
        if (Config.Global.IsLittleEndian)
            Array.Reverse(arrCopy);
        return BitConverter.ToString(arrCopy);
    }

    public static string ToString(byte[] value, int startIndex)
    {
        var arrCopy = value[startIndex..];
        if (Config.Global.IsLittleEndian)
            Array.Reverse(arrCopy);
        return BitConverter.ToString(arrCopy);
    }

    public static bool ToBoolean(byte[] value, int startIndex)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(value);
        return BitConverter.ToBoolean(value, startIndex);
    }

    public static bool ToBoolean(ReadOnlySpan<byte> value)
    {
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            value = value.Reverse();
        return BitConverter.ToBoolean(value);
    }

    public static long DoubleToInt64Bits(double value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToInt64(bytes, 0);
    }

    public static double Int64BitsToDouble(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    public static int SingleToInt32Bits(float value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }

    public static float Int32BitsToSingle(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }

    public static short HalfToInt16Bits(Half value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToInt16(bytes, 0);
    }

    public static Half Int16BitsToHalf(short value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToHalf(bytes, 0);
    }

    public static ulong DoubleToUInt64Bits(double value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt64(bytes, 0);
    }

    public static double UInt64BitsToDouble(ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    public static uint SingleToUInt32Bits(float value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    public static float UInt32BitsToSingle(uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }

    public static ushort HalfToUInt16Bits(Half value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    public static Half UInt16BitsToHalf(ushort value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (Config.Global.IsLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToHalf(bytes, 0);
    }
}
