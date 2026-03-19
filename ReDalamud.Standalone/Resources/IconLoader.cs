namespace ReDalamud.Standalone.Resources;

public enum Icon16 : uint
{
    Accept,
    ArrayType,
    ArrowRefresh,
    ButtonAdd,
    ButtonAddBytes4,
    ButtonAddBytes8,
    ButtonAddBytes64,
    ButtonAddBytes256,
    ButtonAddBytes1024,
    ButtonAddBytes2048,
    ButtonAddBytes4096,
    ButtonAddBytesX,
    ButtonArray,
    ButtonBits,
    ButtonBool,
    ButtonClassAdd,
    ButtonClassInstance,
    ButtonClassPointer,
    ButtonClassRemove,
    ButtonDelete,
    ButtonDouble,
    ButtonDropDown,
    ButtonEnum,
    ButtonFloat,
    ButtonFunction,
    ButtonFunctionPointer,
    ButtonHex8,
    ButtonHex16,
    ButtonHex32,
    ButtonHex64,
    ButtonInsertBytes4,
    ButtonInsertBytes8,
    ButtonInsertBytes64,
    ButtonInsertBytes256,
    ButtonInsertBytes1024,
    ButtonInsertBytes2048,
    ButtonInsertBytes4096,
    ButtonInsertBytesX,
    ButtonInt8,
    ButtonInt16,
    ButtonInt32,
    ButtonInt64,
    ButtonMatrix3x3,
    ButtonMatrix4x4,
    ButtonMatrix3x4,
    ButtonNInt,
    ButtonNUInt,
    ButtonPointer,
    ButtonPointerArray,
    ButtonRemove,
    ButtonText,
    ButtonTextPointer,
    ButtonUInt8,
    ButtonUInt16,
    ButtonUInt32,
    ButtonUInt64,
    ButtonUnion,
    ButtonUText,
    ButtonUTextPointer,
    ButtonVector2,
    ButtonVector3,
    ButtonVector4,
    ButtonVTable,
    Camera,
    CanvasSize,
    Category,
    ChartDelete,
    ClassType,
    ClosedIcon,
    Cogs,
    ColorWheel,
    ControlPause,
    ControlPlay,
    ControlStop,
    CustomType,
    DoubleType,
    DriveGo,
    EnumType,
    Error,
    ExchangeButton,
    Eye,
    FindAccess,
    FindWrite,
    FloatType,
    Folder,
    FunctionType,
    Gear,
    Help,
    Information,
    InterfaceType,
    LeftButton,
    Magnifier,
    MagnifierArrow,
    MagnifierRemove,
    MatrixType,
    OpenIcon,
    PageCode,
    PageCodeAdd,
    PageCodeCpp,
    PageCodeCSharp,
    PageCopy,
    PageWhiteStack,
    Pdb,
    Plugin,
    PointerType,
    Quit,
    Redo,
    RightButton,
    Save,
    SaveAs,
    SettingsEdit,
    SignedType,
    TableGear,
    TextListBullets,
    TextfieldRename,
    TreeCollapse,
    TreeExpand,
    Undo,
    UnsignedType,
    VectorType,
    Warning
}

public enum Icon32 : uint
{
    Glasses,
    Bug,
    CanvasSize,
    Cogs,
    Eye,
    Magnifier,
    PageCode,
    Plugin
}

public class IconLoader
{
    private enum IconType
    {
        Icon16,
        Icon32
    }

    private static readonly Dictionary<ImGuiRenderer.TextureWrap, (IconType Type, uint IconId)> TextureDictionary = [];

    private static bool IsUpperOrNumber(char c) => char.IsUpper(c) || char.IsNumber(c);
    private static bool IsCurrentAndNotLastUpperOrNumber(string s, int i) => IsUpperOrNumber(s[i]) && !(i > 0 && IsUpperOrNumber(s[i - 1]));

    public static ReadOnlySpan<byte> GetIconBytes(Icon16 icon)
    {
        var str = icon.ToString();
        switch (icon)
        {
            case Icon16.ButtonMatrix3x3:
            case Icon16.ButtonMatrix4x4:
            case Icon16.ButtonMatrix3x4:
                var size = str[^3..];
                str = "Button_Matrix_" + size;
                break;
            default:
                for (var i = 1; i < str.Length; i++)
                {
                    if (!IsCurrentAndNotLastUpperOrNumber(str, i)) continue;
                    str = str.Insert(i, "_");
                    i++;
                }
                break;
        }

        return GetResourceBytes($"ReDalamud.Standalone.Resources.Icons.B16x16_{str}.png");
    }

    public static ReadOnlySpan<byte> GetIconBytes(Icon32 icon)
    {
        var str = icon.ToString();
        if (icon == Icon32.Glasses)
            str = "3D_Glasses";
        else
            for (var i = 1; i < str.Length; i++)
            {
                if (!char.IsUpper(str[i]) && !char.IsNumber(str[i])) continue;
                str = str.Insert(i, "_");
                i++;
            }
        return GetResourceBytes($"ReDalamud.Standalone.Resources.Icons.B32x32_{str}.png");
    }

    public static unsafe ImTextureRef GetIconTextureId(Icon16 icon)
    {
        var key = TextureDictionary.Where(t => t.Value.Type == IconType.Icon16).FirstOrDefault(t => t.Value.IconId == (uint)icon).Key;
        if (key != null)
            return key.Texture;
        var textureWrap = new ImGuiRenderer.TextureWrap(GetIconBytes(icon), "PNG");
        TextureDictionary.Add(textureWrap, (IconType.Icon16, (uint)icon));
        return textureWrap.Texture;
    }

    public static unsafe ImTextureRef GetIconTextureId(Icon32 icon)
    {
        var key = TextureDictionary.Where(t => t.Value.Type == IconType.Icon32).FirstOrDefault(t => t.Value.IconId == (uint)icon).Key;
        if (key != null)
            return key.Texture;
        var textureWrap = new ImGuiRenderer.TextureWrap(GetIconBytes(icon), "PNG");
        TextureDictionary.Add(textureWrap, (IconType.Icon32, (uint)icon));
        return textureWrap.Texture;
    }

    public static unsafe void Dispose()
    {
        foreach (var (textureWrap, (_, _)) in TextureDictionary)
        {
            textureWrap.Dispose();
        }
    }
}