using System.Diagnostics.CodeAnalysis;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ReDalamud.Shared.ClientStructs.Data;

public class Data
{
    public string Version;
    public Dictionary<ulong, string> Globals;
    public Dictionary<ulong, string> Functions;
    public Dictionary<string, DataClass> Classes;

    public static bool ParseYaml(string path, ulong baseAddress, [NotNullWhen(true)] out Data? data)
    {
        data = null;
        var file = new FileInfo(path);
        if (!file.Exists) return false;
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(LowerCaseNamingConvention.Instance)
            .Build();
        data = deserializer.Deserialize<Data>(File.ReadAllText(file.FullName));

        // TODO: move this to a different place when application is setup to allow for game to be relaunched
        for (var i = 0; i < data.Classes.Count; i++)
        {
            var (_, value) = data.Classes.ElementAt(i);
            if (value is null or { Instances: null }) continue;
            foreach (var instance in value.Instances)
            {
                instance.Ea -= 0x140000000;
                instance.Ea += baseAddress;
            }
        }

        return true;
    }
}

public class DataClass
{
    public List<DataOffsetDefinition> Instances;
    public List<DataOffsetDefinition> Vtbls;
    public Dictionary<ulong, string> Funcs;
    public Dictionary<ulong, string> Vfuncs;
}

public class DataOffsetDefinition
{
    public ulong Ea;
    public string Base;
    public bool Pointer;
}