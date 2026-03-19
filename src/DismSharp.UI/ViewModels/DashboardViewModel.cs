using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.Core.Helpers;
using DismSharp.Core.Modules;

namespace DismSharp.UI.ViewModels;

/// <summary>磁盘信息显示模型</summary>
public record DiskDisplayInfo(string DriveLetter, string VolumeName, string TotalSize, string FreeSize, double UsagePercent);

/// <summary>仪表盘页面 ViewModel</summary>
public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string _osName = "";

    [ObservableProperty]
    private string _osBuild = "";

    [ObservableProperty]
    private string _machineName = "";

    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _architecture = "";

    [ObservableProperty]
    private string _cpuName = "加载中...";

    [ObservableProperty]
    private string _totalMemory = "加载中...";

    [ObservableProperty]
    private string _availableMemory = "加载中...";

    [ObservableProperty]
    private string _uptime = "";

    [ObservableProperty]
    private string _featureCountText = "...";

    [ObservableProperty]
    private string _driverCountText = "...";

    [ObservableProperty]
    private string _packageCountText = "...";

    [ObservableProperty]
    private bool _isDismLoading = true;

    [ObservableProperty]
    private string _loadingStatus = "正在加载...";

    [ObservableProperty]
    private ObservableCollection<DiskDisplayInfo> _diskDrives = [];

    /// <summary>加载所有仪表盘数据</summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        IsDismLoading = true;
        LoadingStatus = "正在读取系统信息...";

        // 第一阶段：立即显示系统基本信息（零延迟）
        OsName = SystemInfo.GetOsName();
        OsBuild = SystemInfo.GetOsBuild();
        MachineName = SystemInfo.GetMachineName();
        UserName = SystemInfo.GetUserName();
        Architecture = SystemInfo.GetArchitecture();
        Uptime = SystemInfo.FormatUptime(SystemInfo.GetUptime());

        LoadingStatus = "正在读取磁盘信息...";

        // 用 .NET API 获取磁盘（瞬间完成）
        try
        {
            var gcMemInfo = GC.GetGCMemoryInfo();
            TotalMemory = FileHelper.FormatSize(gcMemInfo.TotalAvailableMemoryBytes);

            var drives = System.IO.DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveType == System.IO.DriveType.Fixed)
                .ToList();

            var diskDisplayList = new ObservableCollection<DiskDisplayInfo>();
            foreach (var drive in drives)
            {
                var usagePercent = drive.TotalSize > 0
                    ? (double)(drive.TotalSize - drive.TotalFreeSpace) / drive.TotalSize * 100
                    : 0;
                diskDisplayList.Add(new DiskDisplayInfo(
                    drive.Name.TrimEnd('\\'),
                    drive.VolumeLabel,
                    FileHelper.FormatSize(drive.TotalSize),
                    FileHelper.FormatSize(drive.AvailableFreeSpace),
                    usagePercent));
            }
            DiskDrives = diskDisplayList;
        }
        catch (Exception ex) { Debug.WriteLine($"[Dashboard] Error: {ex.Message}"); }

        // 后台并行：WMI（CPU）+ WMI（可用内存）+ DISM
        LoadingStatus = "正在查询硬件信息...";

        var cpuTask = Task.Run(() =>
        {
            try
            {
                using var searcher = new System.Management.ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                using var results = searcher.Get();
                foreach (var obj in results)
                    return obj["Name"]?.ToString()?.Trim() ?? "未知";
            }
            catch (Exception ex) { Debug.WriteLine($"[Dashboard] Error: {ex.Message}"); }
            return "未知";
        });

        var memTask = Task.Run(() =>
        {
            try
            {
                using var searcher = new System.Management.ManagementObjectSearcher(
                    "SELECT FreePhysicalMemory FROM Win32_OperatingSystem");
                using var results = searcher.Get();
                foreach (var obj in results)
                    return FileHelper.FormatSize((long)(Convert.ToUInt64(obj["FreePhysicalMemory"]) * 1024));
            }
            catch (Exception ex) { Debug.WriteLine($"[Dashboard] Error: {ex.Message}"); }
            return "N/A";
        });

        var dismTask = Task.Run(() =>
        {
            try
            {
                using var session = DismSharpSession.OpenOnline();
                return (
                    Features: session.GetFeatures().Count,
                    Drivers: session.GetDrivers(allDrivers: true).Count,
                    Packages: session.GetPackages().Count,
                    Ok: true
                );
            }
            catch (Exception ex) { Debug.WriteLine($"[Dashboard] Error: {ex.Message}"); }
            return (Features: 0, Drivers: 0, Packages: 0, Ok: false);
        });

        // 先等 WMI 结果（快的先到）
        CpuName = await cpuTask;
        AvailableMemory = await memTask;
        LoadingStatus = "正在查询 DISM 数据...";

        // 等 DISM 结果（最慢）
        var dismResult = await dismTask;
        if (dismResult.Ok)
        {
            FeatureCountText = dismResult.Features.ToString();
            DriverCountText = dismResult.Drivers.ToString();
            PackageCountText = dismResult.Packages.ToString();
        }
        else
        {
            FeatureCountText = "N/A";
            DriverCountText = "N/A";
            PackageCountText = "N/A";
        }

        LoadingStatus = "加载完成";
        IsDismLoading = false;
    }
}
