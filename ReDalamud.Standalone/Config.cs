using System.ComponentModel;
using System.Text;

namespace ReDalamud.Standalone;
public static class Config
{
    public static Styles Styles = new();
    public static Global Global = new();

    public static void Save()
    {
        // Save the config in an ini file
        var sb = new StringBuilder();
        Save(typeof(Styles), Styles, sb);
        Save(typeof(Global), Global, sb);
        File.WriteAllText("config.ini", sb.ToString());
    }

    private static void Save(Type type, object obj, StringBuilder sb)
    {
        var types = new HashSet<Type>();
        sb.AppendLine($"[{type.Name}]");
        foreach (var field in type.GetFields())
        {
            if (types.Add(field.FieldType))
            {
                var doc = field.FieldType.GetDocString();
                if(!string.IsNullOrWhiteSpace(doc))
                    sb.AppendLine($"{doc}");
            }
            var value = field.GetValue(obj);
            if (value == null)
                continue;
            sb.AppendLine($"{field.Name}={value.ToIniString()}");
        }

        sb.AppendLine();
    }

    public static void Load()
    {
        if (!File.Exists("config.ini"))
            return;
        // Load the config from an ini file
        var lines = File.ReadAllLines("config.ini").ToList();

        var styles = new Styles();
        var grouped = new Dictionary<string, List<(string Key, string Value)>>();
        var currentGroup = string.Empty;
        foreach (var line in lines)
        {
            if (line.StartsWith('['))
            {
                currentGroup = line.Trim('[', ']');
                grouped[currentGroup] = new List<(string Key, string Value)>();
            }
            else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith('#'))
            {
                var split = line.Split('=');
                grouped[currentGroup].Add((split[0], split[1]));
            }
        }

        grouped.TryGetValue("Styles", out var styleLines);
        if (styleLines != null)
            foreach (var (key, value) in styleLines)
                Load(Styles, key, value);
        grouped.TryGetValue("Global", out var globalLines);
        if (globalLines != null)
            foreach (var (key, value) in globalLines)
                Load(Global, key, value);
    }

    private static void Load(object obj, string key, string value)
    {
        var field = obj.GetType().GetField(key);
        if (field == null)
            return;
        var val = field.FieldType.FromIniString(value);
        try
        {
            field.SetValue(obj, Convert.ChangeType(val, field.FieldType));
        }
        catch
        {
            field.SetValue(obj, TypeDescriptor.GetConverter(field.FieldType).ConvertFromString(null, null, value));
        }
    }
}

public class Styles
{
    public Color BackgroundColor = Color.FromHex("#373737");
    public Color SelectedColor = Color.FromHex("#707070");
    public Color HiddenColor = Color.FromHex("#6A6A6A");
    public Color AddressColor = Color.FromHex("#00C800");
    public Color IndexColor = Color.FromHex("#20C8C8");
    public Color OffsetColor = Color.FromHex("#FF0000");
    public Color VTableColor = Color.FromHex("#00FF00");
    public Color HexValueColor = Color.FromHex("#E4E4E4");
    public Color CommentColor = Color.FromHex("#00C800");
    public Color TypeColor = Color.FromHex("#8282FF");
    public Color TextColor = Color.FromHex("#3E8BFF");
    public Color NameColor = Color.FromHex("#91E6E6");
    public Color PluginInfoColor = Color.FromHex("#FF00FF");
    public Color ValueColor = Color.FromHex("#FF8000");
}

public class Global
{
    // public bool IsLittleEndian = BitConverter.IsLittleEndian;
    public string ClientStructsPath = "";
}
