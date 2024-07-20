using System.Reflection;

namespace ReDalamud.Standalone.Extensions;
public static class ObjectExtensions
{
    private static readonly Dictionary<Type, Func<object, string, object>> _fromIniStringMethods = new();
    private static readonly Dictionary<Type, Func<object, string>> _toIniStringMethods = new();

    private static void LoadAllMethods()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(object).IsAssignableFrom(p) && 
                        p is { IsSealed: true, IsGenericType: false, IsNested: false, Namespace: not null } 
                        && p.Namespace.StartsWith("ReDalamud")
                        && p.Name != "ObjectExtensions")
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

        foreach (var method in types)
        {
            if (method.Name.StartsWith("FromIniString"))
            {
                _fromIniStringMethods[method.ReturnType] = (obj, str) => method.Invoke(obj, [str])!;
            }

            if (method.Name == "ToIniString")
            {
                _toIniStringMethods[method.GetParameters()[0].ParameterType] = obj => (string)method.Invoke(obj, [obj])!;
            }
        }
    }

    public static string ToIniString(this object obj)
    {
        var type = obj.GetType();
        if (_toIniStringMethods.Count == 0)
            LoadAllMethods();
        return _toIniStringMethods.TryGetValue(type, out var method) ? method(obj) : obj.ToString()!;
    }

    public static object FromIniString(this Type type, string str)
    {
        if (_fromIniStringMethods.Count == 0)
            LoadAllMethods();
        return _fromIniStringMethods.TryGetValue(type, out var method) ? method(type, str) : str;
    }
}
