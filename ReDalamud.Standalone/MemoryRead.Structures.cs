using System.Runtime.InteropServices;

namespace ReDalamud.Standalone;
public partial class MemoryRead
{
    public enum ProcessAccessFlags : uint
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        SYNCHRONIZE = 0x00100000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        PROCESS_ALL_ACCESS = 0x001F0FFF,
        PROCESS_CREATE_PROCESS = 0x0080,
        PROCESS_CREATE_THREAD = 0x0002,
        PROCESS_DUP_HANDLE = 0x0040,
        PROCESS_QUERY_INFORMATION = 0x0400,
        PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
        PROCESS_SET_INFORMATION = 0x0200,
        PROCESS_SET_QUOTA = 0x0100,
        PROCESS_SUSPEND_RESUME = 0x0800,
        PROCESS_TERMINATE = 0x0001,
        PROCESS_VM_OPERATION = 0x0008,
        PROCESS_VM_READ = 0x0010,
        PROCESS_VM_WRITE = 0x0020,
        PROCESS_VM_ALL = PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION,
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public struct MEMORY_BASIC_INFORMATION
    {
        [FieldOffset(0x0)] public nint BaseAddress;
        [FieldOffset(0x8)] public nint AllocationBase;
        [FieldOffset(0x10)] public uint AllocationProtect;
        [FieldOffset(0x14)] public ushort PartitionId;
        [FieldOffset(0x18)] public nint RegionSize;
        [FieldOffset(0x20)] public uint State;
        [FieldOffset(0x24)] public uint Protect;
        [FieldOffset(0x28)] public uint Type;
    }

    public enum MemoryState : uint
    {
        MEM_COMMIT = 0x1000,
        MEM_FREE = 0x10000,
        MEM_RESERVE = 0x2000
    }

    [Flags]
    public enum MemoryProtect : uint
    {
        PAGE_EXECUTE = 0x10,
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READWRITE = 0x40,
        PAGE_EXECUTE_WRITECOPY = 0x80,
        PAGE_NOACCESS = 0x01,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,
        PAGE_TARGETS_INVALID = 0x40000000,
        PAGE_TARGETS_NO_UPDATE = 0x40000000,
        PAGE_GUARD = 0x100,
        PAGE_NOCACHE = 0x200,
        PAGE_WRITECOMBINE = 0x400
    }

    public enum MemoryType : uint
    {
        MEM_IMAGE = 0x1000000,
        MEM_MAPPED = 0x40000,
        MEM_PRIVATE = 0x20000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageDosHeader
    {
        public ushort e_magic;
        public ushort e_cblp;
        public ushort e_cp;
        public ushort e_crlc;
        public ushort e_cparhdr;
        public ushort e_minalloc;
        public ushort e_maxalloc;
        public ushort e_ss;
        public ushort e_sp;
        public ushort e_csum;
        public ushort e_ip;
        public ushort e_cs;
        public ushort e_lfarlc;
        public ushort e_ovno;
        public ushort e_res_0;
        public ushort e_res_1;
        public ushort e_res_2;
        public ushort e_res_3;
        public ushort e_oemid;
        public ushort e_oeminfo;
        public ushort e_res2_0;
        public ushort e_res2_1;
        public ushort e_res2_2;
        public ushort e_res2_3;
        public ushort e_res2_4;
        public ushort e_res2_5;
        public ushort e_res2_6;
        public ushort e_res2_7;
        public ushort e_res2_8;
        public ushort e_res2_9;
        public uint e_lfanew;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageNtHeader
    {
        public uint Signature;
        public ImageFileHeader FileHeader;
        public ImageOptionalHeader OptionalHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageFileHeader
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageOptionalHeader
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;
        public nint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public nint SizeOfStackReserve;
        public nint SizeOfStackCommit;
        public nint SizeOfHeapReserve;
        public nint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;
        public ImageDataDirectory ExportTable;
        public ImageDataDirectory ImportTable;
        public ImageDataDirectory ResourceTable;
        public ImageDataDirectory ExceptionTable;
        public ImageDataDirectory CertificateTable;
        public ImageDataDirectory BaseRelocationTable;
        public ImageDataDirectory Debug;
        public ImageDataDirectory Architecture;
        public ImageDataDirectory GlobalPtr;
        public ImageDataDirectory TLSTable;
        public ImageDataDirectory LoadConfigTable;
        public ImageDataDirectory BoundImport;
        public ImageDataDirectory IAT;
        public ImageDataDirectory DelayImportDescriptor;
        public ImageDataDirectory CLRRuntimeHeader;
        public ImageDataDirectory Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageDataDirectory
    {
        public uint VirtualAddress;
        public uint Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImageSectionHeader
    {
        public fixed byte Name[8];
        public ImageSectionHeaderMisc Misc;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public uint Characteristics;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ImageSectionHeaderMisc
    {
        [FieldOffset(0)] public uint PhysicalAddress;
        [FieldOffset(0)] public uint VirtualSize;
    }

    public struct SectionContainer
    {
        public nint BaseAddress;
        public nint Size;
        public string Name;
        public SectionProtection Protection;
        public SectionType Type;
        public SectionCategory Category;
    }

    [Flags]
    public enum SectionProtection : byte
    {
        NoAccess = 0x0,
        Execute = 0x1,
        Read = 0x2,
        Write = 0x4,
        CopyOnWrite = 0x8,
        Guard = 0x10
    }

    public enum SectionType : byte
    {
        Image = 0x1,
        Mapped = 0x2,
        Private = 0x4
    }

    public enum SectionCategory : byte
    {
        Unknown = 0x0,
        Code = 0x1,
        Data = 0x2,
        Heap = 0x4,
    }
}
