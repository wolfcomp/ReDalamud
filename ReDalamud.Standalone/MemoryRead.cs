using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ReDalamud.Standalone;
public unsafe partial class MemoryRead
{
    public static nint OpenedProcessHandle = nint.Zero;
    public static Process OpenedProcess;
    public static string ProcessName;
    public static List<SectionContainer> MemoryRegions = new();

    [DllImport("kernel32.dll")]
    public static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, void* lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(nint hObject);

    [DllImport("kernel32.dll")]
    public static extern ulong VirtualQueryEx(nint hProcess, nint lpAddress, MEMORY_BASIC_INFORMATION* lpBuffer, uint dwLength);

    public static ulong VirtualQueryEx(nint hProcess, nint lpAddress, out MEMORY_BASIC_INFORMATION buffer)
    {
        var lpBuffer = new MEMORY_BASIC_INFORMATION();
        var result = VirtualQueryEx(hProcess, lpAddress, &lpBuffer, (uint)sizeof(MEMORY_BASIC_INFORMATION));
        buffer = lpBuffer;
        return result;
    }

    public static string IsInRegion(nint address)
    {
        var i = MemoryRegions.FindIndex(region => address >= region.BaseAddress && address < region.BaseAddress + region.Size);
        return i == -1 ? "" : !string.IsNullOrWhiteSpace(MemoryRegions[i].Name) ? MemoryRegions[i].Name : MemoryRegions[i].Category.ToString();
    }

    public static void ScanAllProcessMemoryRegions()
    {
        MemoryRegions.Clear();
        nint address = 0;
        nint size = 0x1000;
        while (VirtualQueryEx(OpenedProcessHandle, address, out var buffer) != 0 && address + size > address)
        {
            if (buffer is { State: (uint)MemoryState.MEM_COMMIT, Type: (uint)MemoryType.MEM_PRIVATE })
            {
                var section = new SectionContainer { BaseAddress = address, Size = buffer.RegionSize };

                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_EXECUTE) == MemoryProtect.PAGE_EXECUTE) section.Protection |= SectionProtection.Execute;
                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_EXECUTE_READ) == MemoryProtect.PAGE_EXECUTE_READ) section.Protection |= SectionProtection.Execute | SectionProtection.Read;
                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_EXECUTE_READWRITE) == MemoryProtect.PAGE_EXECUTE_READWRITE) section.Protection |= SectionProtection.Execute | SectionProtection.Read | SectionProtection.Write;
                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_EXECUTE_WRITECOPY) == MemoryProtect.PAGE_EXECUTE_WRITECOPY) section.Protection |= SectionProtection.Execute | SectionProtection.Read | SectionProtection.CopyOnWrite;
                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_READONLY) == MemoryProtect.PAGE_READONLY) section.Protection |= SectionProtection.Read;
                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_READWRITE) == MemoryProtect.PAGE_READWRITE) section.Protection |= SectionProtection.Read | SectionProtection.Write;
                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_WRITECOPY) == MemoryProtect.PAGE_WRITECOPY) section.Protection |= SectionProtection.Read | SectionProtection.CopyOnWrite;
                if (((MemoryProtect)buffer.Protect & MemoryProtect.PAGE_GUARD) == MemoryProtect.PAGE_GUARD) section.Protection |= SectionProtection.Guard;

                section.Type = (MemoryType)buffer.Type switch
                {
                    MemoryType.MEM_IMAGE => SectionType.Image,
                    MemoryType.MEM_MAPPED => SectionType.Mapped,
                    MemoryType.MEM_PRIVATE => SectionType.Private,
                    _ => section.Type
                };

                section.Category = section.Type == SectionType.Private ? SectionCategory.Heap : SectionCategory.Unknown;

                MemoryRegions.Add(section);
            }
            size = buffer.RegionSize;
            address += size;
        }

        MapSectionContainers();
    }

    public static void MapSectionContainers()
    {
        var baseAddress = GetOpenedProcessAddress();

        if (!Read(baseAddress, out ImageDosHeader dosHeader) ||
            !Read(baseAddress + (nint)dosHeader.e_lfanew, out ImageNtHeader ntHeader))
            return;

        var sections = new ImageSectionHeader[ntHeader.FileHeader.NumberOfSections];
        var sectionSize = Unsafe.SizeOf<ImageSectionHeader>();
        fixed (ImageSectionHeader* p = &sections[0])
            ReadMemory(baseAddress + (nint)dosHeader.e_lfanew + 264, p, sectionSize * sections.Length);
        for (var i = 0; i < sections.Length; i++)
        {
            processSection(ref sections[i]);
        }

        return;

        void processSection(ref ImageSectionHeader header)
        {
            var vAddress = (nint)header.VirtualAddress;
            var sectionIndex = MemoryRegions.FindIndex(t => t.BaseAddress == baseAddress + vAddress);
            var section = sectionIndex == -1 ? new SectionContainer { BaseAddress = baseAddress + vAddress, Size = (nint)header.Misc.VirtualSize } : MemoryRegions[sectionIndex];
            var len = 0;
            var bytes = new byte[8];
            while (header.Name[len] != '\0')
            {
                bytes[len] = header.Name[len];
                len++;
            }
            fixed (byte* p = bytes)
                section.Name = Encoding.UTF8.GetString(p, len);
            if ((header.Characteristics & 0x20) == 0x20) section.Category = SectionCategory.Code;
            else if ((header.Characteristics & 0xC0) != 0) section.Category = SectionCategory.Data;
            if(sectionIndex == -1)
                MemoryRegions.Add(section);
            else
                MemoryRegions[sectionIndex] = section;
        }
    }

    public static nint OpenProcess(string processName)
    {
        return OpenProcess(processName, ProcessAccessFlags.PROCESS_VM_ALL);
    }

    public static nint OpenProcess(string processName, ProcessAccessFlags flags)
    {
        var process = Process.GetProcessesByName(processName).FirstOrDefault();

        OpenedProcess = process ?? throw new Exception("Process not found");
        ProcessName = processName;
        OpenedProcessHandle = OpenProcess((uint)flags, false, process.Id);
        ScanAllProcessMemoryRegions();

        return OpenedProcessHandle;
    }

    public static byte[] ReadBytes(nint address, int size)
    {
        var buffer = new byte[size];
        fixed (byte* ptr = buffer)
            ReadMemory(address, ptr, size);
        return buffer;
    }

    public static byte[] ReadPointerBytes(nint address, int size)
    {
        var pointer = BitConverter.ToInt64(ReadBytes(address, 8));
        return ReadBytes((nint)pointer, size);
    }

    public static bool ReadPointer<T>(nint address, out T value) where T : unmanaged
    {
        var pointer = BitConverter.ToInt64(ReadBytes(address, 8));
        return Read((nint)pointer, out value);
    }

    public static bool Read<T>(nint address, out T value) where T : unmanaged
    {
        var ret = new T();
        var b = ReadMemory(address, &ret, sizeof(T));
        value = ret;
        return b;
    }

    public static bool ReadMemory(nint address, void* buffer, int size)
    {
        var numRead = 0;
        var read = ReadProcessMemory(OpenedProcessHandle, address, buffer, size, ref numRead);
        if (numRead != size)
            throw new Exception("Failed to read memory");
        return read;
    }

    public static string CharFromBytes(byte[] bytes) => string.Join("", bytes.Select(t => t < 20 ? '.' : (char)t));

    public static string Utf8FromBytePtr(byte* ptr)
    {
        var len = 0;
        while (ptr[len] != '\0')
            len++;
        return Encoding.UTF8.GetString(ptr, len);
    }

    public static nint GetOpenedProcessAddress()
    {
        return OpenedProcess.MainModule?.BaseAddress ?? nint.Zero;
    }

    public static void Dispose()
    {
        if (OpenedProcessHandle != nint.Zero)
            CloseHandle(OpenedProcessHandle);
    }
}
