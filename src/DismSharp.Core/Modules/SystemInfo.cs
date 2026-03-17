using System.Runtime.InteropServices;

namespace DismSharp.Core.Modules;

/// <summary>系统信息采集器，提供操作系统和运行环境基本信息</summary>
public static class SystemInfo
{
    /// <summary>获取操作系统描述（如 "Microsoft Windows 11 Pro"）</summary>
    public static string GetOsName() => RuntimeInformation.OSDescription;

    /// <summary>获取操作系统版本号（如 "10.0.22631.0"）</summary>
    public static string GetOsBuild() => Environment.OSVersion.Version.ToString();

    /// <summary>获取计算机名称</summary>
    public static string GetMachineName() => Environment.MachineName;

    /// <summary>获取当前用户名</summary>
    public static string GetUserName() => Environment.UserName;

    /// <summary>获取系统运行时间</summary>
    public static TimeSpan GetUptime() => TimeSpan.FromMilliseconds(Environment.TickCount64);

    /// <summary>格式化运行时间为可读字符串</summary>
    public static string FormatUptime(TimeSpan uptime) =>
        uptime.Days > 0
            ? $"{uptime.Days} 天 {uptime.Hours} 小时 {uptime.Minutes} 分钟"
            : $"{uptime.Hours} 小时 {uptime.Minutes} 分钟";

    /// <summary>获取系统架构（如 "X64"）</summary>
    public static string GetArchitecture() => RuntimeInformation.OSArchitecture.ToString();

    /// <summary>获取 .NET 运行时版本</summary>
    public static string GetDotNetVersion() => RuntimeInformation.FrameworkDescription;
}
