using System.Management;

namespace DismSharp.Core.Helpers;

/// <summary>WMI 查询辅助工具，用于获取硬件信息</summary>
public static class WmiHelper
{
    /// <summary>磁盘驱动器信息</summary>
    public record DiskDriveInfo(string DriveLetter, string VolumeName, ulong TotalBytes, ulong FreeBytes);

    /// <summary>获取 CPU 名称</summary>
    public static async Task<string> GetCpuNameAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                using var results = searcher.Get();
                foreach (var obj in results)
                {
                    return obj["Name"]?.ToString()?.Trim() ?? "未知";
                }
            }
            catch
            {
                // WMI 查询失败时返回默认值
            }
            return "未知";
        }).ConfigureAwait(false);
    }

    /// <summary>获取内存信息（总量和可用量，单位字节）</summary>
    public static async Task<(ulong TotalBytes, ulong AvailableBytes)> GetMemoryInfoAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
                using var results = searcher.Get();
                foreach (var obj in results)
                {
                    // WMI 返回的单位是 KB，需转换为字节
                    var totalKb = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                    var freeKb = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                    return (totalKb * 1024, freeKb * 1024);
                }
            }
            catch
            {
                // WMI 查询失败时返回默认值
            }
            return (0UL, 0UL);
        }).ConfigureAwait(false);
    }

    /// <summary>获取逻辑磁盘驱动器信息</summary>
    public static async Task<List<DiskDriveInfo>> GetDiskDrivesAsync()
    {
        return await Task.Run(() =>
        {
            var result = new List<DiskDriveInfo>();
            try
            {
                // DriveType=3 表示本地磁盘
                using var searcher = new ManagementObjectSearcher(
                    "SELECT DeviceID, VolumeName, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType=3");
                using var results = searcher.Get();
                foreach (var obj in results)
                {
                    var driveLetter = obj["DeviceID"]?.ToString() ?? "";
                    var volumeName = obj["VolumeName"]?.ToString() ?? "";
                    var totalBytes = Convert.ToUInt64(obj["Size"] ?? 0);
                    var freeBytes = Convert.ToUInt64(obj["FreeSpace"] ?? 0);
                    result.Add(new DiskDriveInfo(driveLetter, volumeName, totalBytes, freeBytes));
                }
            }
            catch
            {
                // WMI 查询失败时返回空列表
            }
            return result;
        }).ConfigureAwait(false);
    }
}
