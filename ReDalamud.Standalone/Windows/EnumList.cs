using ReDalamud.Standalone.Types;
using System.Reflection;

namespace ReDalamud.Standalone.Windows;

internal class EnumList
{
    private static  List<(string Name, Dictionary<string, nuint> values)> _loadedFfxivClientStructTypes = [];
    private static bool _processingXivTypes = true;
    private static float _spinner = 0;

    public static void InitTypes()
    {
        _loadedFfxivClientStructTypes = GetLoadedFFXIVClientStructTypes();
        _processingXivTypes = false;
    }

    public static void Draw()
    {
        ImGui.Begin("EnumList");
        if(_processingXivTypes)
        {
            ImGui.Text("Processing FFXIV Types...");
            ImGuiExt.SpinnerDots("ClasListSpinner", ref _spinner, 16, 4, Color.FromRGBFloat(1,1,1));
            ImGui.End();
            return;
        }
        var clipper = ImGui.ImGuiListClipper();
        clipper.Begin(_loadedFfxivClientStructTypes.Count, ImGui.GetTextLineHeightWithSpacing());
        while (clipper.Step())
        {
            for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
            {
                if (i >= _loadedFfxivClientStructTypes.Count)
                    break;
                if (i < 0)
                    continue;
                var (name, _) = _loadedFfxivClientStructTypes[i];
                ImGui.Text(name);
            }
        }
        clipper.End();
        clipper.Destroy();
        ImGui.End();
    }

    public static List<(string Name, Dictionary<string, nuint> values)> GetLoadedFFXIVClientStructTypes()
    {
        var types = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        if(assemblies.All(t => t.ManifestModule.Name != "FFXIVClientStructs.dll"))
            assemblies = assemblies.Append(Assembly.LoadFrom("FFXIVClientStructs.dll")).ToArray();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                var nm = type.Namespace?.Split('.') ?? [];
                if (nm.Length > 1 && nm[0] == "FFXIVClientStructs" && nm[1] is "FFXIV" && type is { IsEnum: true })
                {
                    types.Add(type);
                }
            }
        }

        return types.Select(t =>
        {
            var name = GetNameFromType(t);
            return (name, new Dictionary<string, nuint>());
        }).OrderBy(t => t.name).ToList();
    }

    private static string GetNameFromType(Type type)
    {
        if (type.DeclaringType != null)
        {
            return GetNameFromType(type.DeclaringType) + "::" + type.Name;
        }
        return type.Namespace![25..].Replace(".", "::") + "::" + type.Name;
    }
}