using ReDalamud.Shared.ClientStructs.Data;
using ReDalamud.Standalone.Types;
using System.Reflection;

namespace ReDalamud.Standalone.Windows;

public class ClassList
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private static List<ClassRenderer> _loadedFfxivClientStructTypes = [];
    private static List<int> _hasInstances = [];
    private static bool _processingXivTypes = true;
    private static float _spinner = 0;

    public static void InitTypes()
    {
        _loadedFfxivClientStructTypes = GetLoadedFFXIVClientStructTypes();
        _processingXivTypes = false;

        if (!string.IsNullOrWhiteSpace(Config.Global.ClientStructsPath) && Data.ParseYaml(Config.Global.ClientStructsPath, (ulong)MemoryRead.OpenedProcess.MainModule!.BaseAddress, out var data))
        {
            ClientStructsData = data;
            CheckClientStructsInstances();
        }
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
                var type = _loadedFfxivClientStructTypes[i];
                var size = ImGui.CalcTextSize(type.Name);
                var cursorPos = ImGui.GetCursorPos();
                if (ImGui.Selectable(type.Name, StaticClassView.CurrentClassView == type, ImGuiSelectableFlags.SpanAllColumns))
                {
                    StaticClassView.CurrentClassView = type;
                }

                if (!_hasInstances.Contains(i))
                {
                    continue;
                }

                cursorPos.X += size.X;
                ImGui.SetCursorPos(cursorPos);
                ImGui.PushStyleColor(ImGuiCol.Text, Config.Styles.CommentColor.InternalValue);
                ImGui.TextUnformatted(" [Instance]");
                ImGui.PopStyleColor();
            }
        }
        clipper.End();
        clipper.Destroy();
        ImGui.End();
    }

    public static void CheckClientStructsInstances()
    {
        if (ClientStructsData == null || _loadedFfxivClientStructTypes.Count == 0) return;
        _hasInstances.Clear();
        foreach (var item in ClientStructsData.Classes)
        {
            var typeIndex = _loadedFfxivClientStructTypes.FindIndex(t => t.Name == item.Key);
            if (typeIndex < 0 || item.Value is null or { Instances: null or { Count: 0 } }) continue;
            _loadedFfxivClientStructTypes[typeIndex].Address = (nint)item.Value.Instances[0].Ea;
            _loadedFfxivClientStructTypes[typeIndex].IsPointerAddress = item.Value.Instances[0].Pointer;
            _hasInstances.Add(typeIndex);
        }
    }

    private static List<ClassRenderer> GetLoadedFFXIVClientStructTypes()
    {
        var types = new List<(Type Type, int Size)>();
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
                    types.Add((type, type.StructLayoutAttribute!.Size));
                }
            }
        }

        return types.Select(t => new ClassRenderer(t.Size)
        {
            Name = GetNameFromType(t.Type),
        }).OrderBy(t => t.Name).ToList();
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