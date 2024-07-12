using System.Reflection;

namespace ReDalamud.Standalone.Utils;

public static class Util
{
    public static string GetResourceString(string path)
    {
        using var stream = GetResourceStream(path);
        if (stream == null) return string.Empty;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static byte[]? GetResourceBytes(string path)
    {
        using var stream = GetResourceStream(path);
        if (stream == null) return null;
        var buffer = new byte[stream.Length];
        var read = stream.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    public static Stream? GetResourceStream(string path)
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
    }

    public static Dictionary<string, (string Vertex, string Fragment)> GetEmbeddedShaderFiles()
    {
        var shaders = new Dictionary<string, (string Vertex, string Fragment)>();
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var name in assembly.GetManifestResourceNames())
        {
            if (!name.StartsWith("ReDalamud.Standalone.Shaders.")) continue;
            var parts = name.Split('.');
            var shaderName = parts[^2];
            var type = parts[^1];
            var (vertex, fragment) = shaders.GetOrAdd(shaderName, _ => (string.Empty, string.Empty));
            var shaderText = GetResourceString(name);
            switch (type)
            {
                case "vs":
                    vertex = shaderText;
                    break;
                case "fs":
                    fragment = shaderText;
                    break;
            }
            shaders[shaderName] = (vertex, fragment);
        }

        return shaders;
    }

    public static T GetOrAdd<T>(this Dictionary<string, T> dict, string key, Func<string, T> factory)
    {
        if (dict.TryGetValue(key, out var value)) return value;
        value = factory(key);
        dict[key] = value;
        return value;
    }
}