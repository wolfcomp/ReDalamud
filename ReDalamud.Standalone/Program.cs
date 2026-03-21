using ReDalamud.Shared.ClientStructs.Data;
using ReDalamud.Standalone.Types;

namespace ReDalamud.Standalone;

public class Program
{
    private static Timer? _timer;
    public static Random Rand = new();
    public static bool ShouldSaveOnFrame = false;
    public static Data? ClientStructsData = null;

    public static unsafe void Main(string[] args)
    {
        var loc = Directory.GetCurrentDirectory();
        DockWindow.IsFirstSetup = !File.Exists(Path.Combine(loc, "imgui.ini"));

        try
        {
            ImGuiRenderer.CreateWindow("ReDalamud.Standalone", 800, 600);
        }
        catch (Exception e) {
            Console.Error.WriteLine(e);
        }
        Config.Load();

        var quit = false;

        var process = MemoryRead.OpenProcess("ffxiv_dx11");

        if (process != nint.Zero)
        {
            Console.WriteLine("Opened process successfully");
            StaticClassView.CurrentClassView = new ClassRenderer(MemoryRead.GetOpenedProcessAddress());
            _timer = new Timer(_ => MemoryRead.ScanAllProcessMemoryRegions(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            Task.Run(ClassList.InitTypes);
            Task.Run(EnumList.InitTypes);
        }
        else
        {
            Console.WriteLine("Failed to open process");
        }

        var imGuiCols = Enum.GetValues<ImGuiCol>().Where(t => t != ImGuiCol.Count).ToList();
        var imGuiStyleVars = Enum.GetValues<ImGuiStyleVar>().Where(t => t != ImGuiStyleVar.Count).ToList();
        foreach (var imGuiCol in imGuiCols)
        {
            unsafe
            {
                var col = ImGui.GetStyleColorVec4(imGuiCol);
                ImGuiSmrt.Color.Stack.Add((imGuiCol, *col));
            }
        }

        foreach (var imGuiStyleVar in imGuiStyleVars)
        {
            var vec = ImGuiExt.GetStyleObject(imGuiStyleVar);
            ImGuiSmrt.Style.Stack.Add((imGuiStyleVar, vec));
        }

        while (!ImGuiRenderer.Instance.ProcessExit())
        {
            ImGuiRenderer.Instance.NewFrame();
            MainMenuBar.Draw();
            DockWindow.Draw();
            ToolBar.Draw();
            ClassList.Draw();
            EnumList.Draw();
            StaticClassView.Draw();
            ConfigWindow.Draw();
            ImGuiRenderer.Instance.Render();

            if (!ShouldSaveOnFrame)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(Config.Global.ClientStructsPath) && Data.ParseYaml(Config.Global.ClientStructsPath, (ulong)MemoryRead.OpenedProcess.MainModule!.BaseAddress, out var data))
            {
                ClientStructsData = data;
                ClassList.CheckClientStructsInstances();
            }

            Config.Save();
            ShouldSaveOnFrame = false;
        }

        ImGuiRenderer.Instance.Dispose();
        _timer?.Dispose();
        MemoryRead.Dispose();
        SDL.Quit();
    }
}