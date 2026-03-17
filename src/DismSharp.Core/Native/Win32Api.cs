using System.Runtime.InteropServices;

namespace DismSharp.Core.Native;

/// <summary>Win32 API P/Invoke 声明</summary>
internal static partial class Win32Api
{
    /// <summary>清空回收站</summary>
    /// <param name="hwnd">窗口句柄（可为 IntPtr.Zero）</param>
    /// <param name="pszRootPath">驱动器根路径（null 清空所有驱动器）</param>
    /// <param name="dwFlags">标志位</param>
    [LibraryImport("shell32.dll", EntryPoint = "SHEmptyRecycleBinW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

    /// <summary>查询回收站信息</summary>
    [LibraryImport("shell32.dll", EntryPoint = "SHQueryRecycleBinW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int SHQueryRecycleBin(string? pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);

    // SHEmptyRecycleBin 标志
    public const uint SHERB_NOCONFIRMATION = 0x00000001;
    public const uint SHERB_NOPROGRESSUI = 0x00000002;
    public const uint SHERB_NOSOUND = 0x00000004;

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SHQUERYRBINFO
    {
        public uint cbSize;
        public long i64Size;
        public long i64NumItems;
    }
}
