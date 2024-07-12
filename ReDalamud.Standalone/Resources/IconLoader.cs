using ReDalamud.Standalone.Utils;
using System.Runtime.InteropServices;

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

    private static Dictionary<uint, (IconType Type, uint IconId, nint Handle)> _textureDictionary = new();

    public static byte[]? GetIconBytes(Icon16 icon)
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
                    if (!char.IsUpper(str[i]) && !char.IsNumber(str[i])) continue;
                    str = str.Insert(i, "_");
                    i++;
                }
                break;
        }

        return Util.GetResourceBytes($"ReDalamud.Standalone.Resources.Icons.B16x16_{str}.png");
    }

    public static byte[]? GetIconBytes(Icon32 icon)
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
        return Util.GetResourceBytes($"ReDalamud.Standalone.Resources.Icons.B32x32_{str}.png");
    }

    public static unsafe nint GetIconTextureId(Icon16 icon)
    {
        var key = _textureDictionary.Where(t => t.Value.Type == IconType.Icon16).FirstOrDefault(t => t.Value.IconId == (uint)icon).Key;
        if (key != 0)
            return (nint)key;
        var bytes = GetIconBytes(icon);
        if (bytes == null)
            return 0;
        nint surf;
        fixed (byte* ptr = bytes)
        {
            surf = IMG_Load_RW(SDL_RWFromMem((nint)ptr, bytes.Length), 1);
        }
        if (surf == nint.Zero)
            throw new Exception("Failed to load image.");
        var s = (byte*)((SDL_Surface*)surf)->pixels;
        var height = ((SDL_Surface*)surf)->h;
        var width = ((SDL_Surface*)surf)->w;

        var textureId = ImGuiRenderer.LoadTexture((nint)s, width, height);
        _textureDictionary[textureId] = (IconType.Icon16, (uint)icon, surf);
        return (nint)textureId;
    }

    public static unsafe nint GetIconTextureId(Icon32 icon)
    {
        var key = _textureDictionary.Where(t => t.Value.Type == IconType.Icon32).FirstOrDefault(t => t.Value.IconId == (uint)icon).Key;
        if (key != 0)
            return (nint)key;
        var bytes = GetIconBytes(icon);
        if (bytes == null)
            return 0;
        nint surf;
        fixed (byte* ptr = bytes)
        {
            surf = IMG_Load_RW(SDL_RWFromMem((nint)ptr, bytes.Length), 1);
        }
        if (surf == nint.Zero)
            throw new Exception("Failed to load image.");
        var s = (byte*)((SDL_Surface*)surf)->pixels;
        var height = ((SDL_Surface*)surf)->h;
        var width = ((SDL_Surface*)surf)->w;

        var textureId = ImGuiRenderer.LoadTexture((nint)s, width, height);
        _textureDictionary[textureId] = (IconType.Icon32, (uint)icon, surf);
        return (nint)textureId;
    }

    public static void Dispose()
    {
        foreach (var (textureId, (_, _, handle)) in _textureDictionary)
        {
            GL.DeleteTexture(textureId);
            SDL_FreeSurface(handle);
        }
    }
}