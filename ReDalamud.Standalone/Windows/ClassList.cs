using System.Reflection;

namespace ReDalamud.Standalone.Windows;

public class ClassList
{
    static List<Type> _loadedFfxivClientStructTypes = GetLoadedFFXIVClientStructTypes();

    public static void Draw()
    {
        ImGui.Begin("ClassList");
        _loadedFfxivClientStructTypes.ForEach(type =>
        {
            if (ImGui.Selectable(type.Name))
            {
            }
        });
        ImGui.End();
    }

    public static List<Type> GetLoadedFFXIVClientStructTypes()
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
                if (nm.Length > 1 && nm[0] == "FFXIVClientStructs" && nm[1] is "FFXIV" or "Havok" or "STD" && type is { IsValueType: true, IsPrimitive: false, IsEnum: false } && !type.Name.Contains("VirtualTable") && !type.Name.Contains("e__FixedBuffer"))
                {
                    types.Add(type);
                }
            }
        }

        return types;
    }
}