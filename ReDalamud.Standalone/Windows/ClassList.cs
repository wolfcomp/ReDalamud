using ReDalamud.Standalone.Types;
using System.Reflection;

namespace ReDalamud.Standalone.Windows;

public class ClassList
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private static List<ClassRenderer> _loadedFfxivClientStructTypes = [];
    private static bool _processingXivTypes = true;
    private static float _spinner = 0;

    public static void InitTypes()
    {
        _loadedFfxivClientStructTypes = GetLoadedFFXIVClientStructTypes();
        _processingXivTypes = false;
    }

    public static unsafe void Draw()
    {
        ImGui.Begin("ClassList");
        if(_processingXivTypes)
        {
            ImGui.Text("Processing FFXIV Types...");
            ImGuiExt.SpinnerDots("ClasListSpinner", ref _spinner, 16, 4, Color.FromRGBFloat(1,1,1));
            ImGui.End();
            return;
        }
        ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
        clipper.Begin(_loadedFfxivClientStructTypes.Count, ImGui.GetTextLineHeightWithSpacing());
        while (clipper.Step())
        {
            for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
            {
                if (i >= _loadedFfxivClientStructTypes.Count)
                    break;
                if (i < 0)
                    continue;
                var type = _loadedFfxivClientStructTypes[i];
                if (type.DrawName(StaticClassView.CurrentClassView == type))
                {
                    StaticClassView.CurrentClassView = type;
                }
            }
        }
        clipper.End();
        clipper.Destroy();
        ImGui.End();
    }

    public static List<ClassRenderer> GetLoadedFFXIVClientStructTypes()
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
                if (nm.Length > 1 && nm[0] == "FFXIVClientStructs" && nm[1] is "FFXIV" && type is { IsValueType: true, IsPrimitive: false, IsEnum: false } && !type.Name.Contains("VirtualTable") && !type.Name.Contains("e__FixedBuffer"))
                {
                    types.Add(type);
                }
            }
        }

        return types.Select(t => new ClassRenderer
        {
            Name = t.Namespace![25..].Replace(".", "::") + "::" + t.Name,
        }).OrderBy(t => t.Name).ToList();
    }
}