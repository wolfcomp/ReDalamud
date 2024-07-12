using System.Runtime.InteropServices;

namespace ReDalamud.Standalone;
public class MemoryRead
{
    public static nint OpenedProcessHandle = nint.Zero;
    public static Process OpenedProcess;
    public static string ProcessName;

    [DllImport("kernel32.dll")]
    public static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(nint hObject);

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

        return OpenedProcessHandle;
    }

    public static byte[] ReadBytes(nint hProcess, nint address, int size)
    {
        var buffer = new byte[size];
        ReadProcessMemory(hProcess, address, buffer, buffer.Length, ref size);
        return buffer;
    }

    public static byte[] ReadBytes(nint address, int size)
    {
        return ReadBytes(OpenedProcessHandle, address, size);
    }

    public static byte[] ReadPointerBytes(nint address, int size)
    {
        var pointer = BitConverter.ToInt64(ReadBytes(address, 8), 0);
        return ReadBytes((nint)pointer, size);
    }

    public static nint GetOpenedProcessAddress()
    {
        return OpenedProcess.MainModule?.BaseAddress ?? nint.Zero;
    }

    public static void Dispose()
    {
        if(OpenedProcessHandle != nint.Zero)
            CloseHandle(OpenedProcessHandle);
    }

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
}
