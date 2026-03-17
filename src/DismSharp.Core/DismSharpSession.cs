using System.Runtime.InteropServices;
using DismSharp.Core.Native;

namespace DismSharp.Core;

/// <summary>功能信息</summary>
/// <param name="FeatureName">功能名称</param>
/// <param name="State">功能状态</param>
public record FeatureInfo(string FeatureName, DismPackageFeatureState State);

/// <summary>驱动包信息</summary>
/// <param name="PublishedName">发布名称</param>
/// <param name="OriginalFileName">原始文件名</param>
/// <param name="InBox">是否为系统内置</param>
/// <param name="ClassName">驱动类名</param>
/// <param name="ClassDescription">驱动类描述</param>
/// <param name="ProviderName">提供商名称</param>
/// <param name="DriverSignature">签名状态</param>
/// <param name="BootCritical">是否为启动关键驱动</param>
/// <param name="Date">驱动日期</param>
/// <param name="DriverVersion">驱动版本</param>
public record DriverPackageInfo(
    string PublishedName,
    string OriginalFileName,
    bool InBox,
    string ClassName,
    string ClassDescription,
    string ProviderName,
    DismDriverSignature DriverSignature,
    bool BootCritical,
    DateTime Date,
    string DriverVersion);

/// <summary>包基本信息</summary>
/// <param name="PackageName">包名称</param>
/// <param name="PackageState">包状态</param>
/// <param name="ReleaseType">发布类型</param>
/// <param name="InstallTime">安装时间</param>
public record PackageBasicInfo(
    string PackageName,
    DismPackageFeatureState PackageState,
    DismReleaseType ReleaseType,
    DateTime InstallTime);

/// <summary>DISM 会话管理器，封装 DISM API 的初始化、会话和查询操作</summary>
public sealed class DismSharpSession : IDisposable
{
    private readonly uint _session;
    private bool _disposed;
    private static bool _initialized;

    /// <summary>内部会话句柄，供同程序集内的模块使用</summary>
    internal uint SessionHandle => _session;

    private DismSharpSession(uint session)
    {
        _session = session;
    }

    /// <summary>初始化 DISM API 引擎</summary>
    /// <param name="logLevel">日志级别</param>
    /// <param name="logFilePath">日志文件路径（可选）</param>
    /// <param name="scratchDirectory">临时目录（可选）</param>
    public static void Initialize(
        DismLogLevel logLevel = DismLogLevel.DismLogErrors,
        string? logFilePath = null,
        string? scratchDirectory = null)
    {
        if (_initialized) return;

        int hr = DismApi.DismInitialize(logLevel, logFilePath, scratchDirectory);
        DismSharpException.ThrowIfFailed(hr);
        _initialized = true;
    }

    /// <summary>关闭 DISM API 引擎</summary>
    public static void Shutdown()
    {
        if (!_initialized) return;

        int hr = DismApi.DismShutdown();
        _initialized = false;
        DismSharpException.ThrowIfFailed(hr);
    }

    /// <summary>打开在线系统映像会话</summary>
    /// <returns>DISM 会话实例</returns>
    public static DismSharpSession OpenOnline()
    {
        EnsureInitialized();
        int hr = DismApi.DismOpenSession(DismApi.DISM_ONLINE_IMAGE, null, null, out uint session);
        DismSharpException.ThrowIfFailed(hr);
        return new DismSharpSession(session);
    }

    /// <summary>打开离线映像会话</summary>
    /// <param name="imagePath">映像挂载路径</param>
    /// <param name="windowsDirectory">Windows 目录（可选）</param>
    /// <param name="systemDrive">系统驱动器（可选）</param>
    /// <returns>DISM 会话实例</returns>
    public static DismSharpSession OpenOffline(
        string imagePath,
        string? windowsDirectory = null,
        string? systemDrive = null)
    {
        EnsureInitialized();
        int hr = DismApi.DismOpenSession(imagePath, windowsDirectory, systemDrive, out uint session);
        DismSharpException.ThrowIfFailed(hr);
        return new DismSharpSession(session);
    }

    /// <summary>获取所有 Windows 功能列表</summary>
    /// <returns>功能信息列表</returns>
    public List<FeatureInfo> GetFeatures()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int hr = DismApi.DismGetFeatures(
            _session, null, DismPackageIdentifier.DismPackageNone,
            out IntPtr featuresPtr, out uint count);
        DismSharpException.ThrowIfFailed(hr);

        try
        {
            var result = new List<FeatureInfo>((int)count);
            int structSize = Marshal.SizeOf<DismFeature>();
            for (int i = 0; i < count; i++)
            {
                var feature = Marshal.PtrToStructure<DismFeature>(featuresPtr + i * structSize);
                string name = Marshal.PtrToStringUni(feature.FeatureName) ?? "";
                result.Add(new FeatureInfo(name, feature.State));
            }
            return result;
        }
        finally
        {
            DismApi.DismDelete(featuresPtr);
        }
    }

    /// <summary>获取驱动列表</summary>
    /// <param name="allDrivers">true 获取所有驱动（包括内置），false 仅第三方驱动</param>
    /// <returns>驱动包信息列表</returns>
    public List<DriverPackageInfo> GetDrivers(bool allDrivers = false)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int hr = DismApi.DismGetDrivers(_session, allDrivers, out IntPtr driversPtr, out uint count);
        DismSharpException.ThrowIfFailed(hr);

        try
        {
            var result = new List<DriverPackageInfo>((int)count);
            int structSize = Marshal.SizeOf<DismDriverPackage>();
            for (int i = 0; i < count; i++)
            {
                var driver = Marshal.PtrToStructure<DismDriverPackage>(driversPtr + i * structSize);
                result.Add(new DriverPackageInfo(
                    Marshal.PtrToStringUni(driver.PublishedName) ?? "",
                    Marshal.PtrToStringUni(driver.OriginalFileName) ?? "",
                    driver.IsInBox,
                    Marshal.PtrToStringUni(driver.ClassName) ?? "",
                    Marshal.PtrToStringUni(driver.ClassDescription) ?? "",
                    Marshal.PtrToStringUni(driver.ProviderName) ?? "",
                    driver.DriverSignature,
                    driver.IsBootCritical,
                    driver.Date.ToDateTime(),
                    $"{driver.MajorVersion}.{driver.MinorVersion}.{driver.Build}.{driver.Revision}"));
            }
            return result;
        }
        finally
        {
            DismApi.DismDelete(driversPtr);
        }
    }

    /// <summary>获取已安装的更新包列表</summary>
    /// <returns>包基本信息列表</returns>
    public List<PackageBasicInfo> GetPackages()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int hr = DismApi.DismGetPackages(_session, out IntPtr packagesPtr, out uint count);
        DismSharpException.ThrowIfFailed(hr);

        try
        {
            var result = new List<PackageBasicInfo>((int)count);
            int structSize = Marshal.SizeOf<DismPackage>();
            for (int i = 0; i < count; i++)
            {
                var package = Marshal.PtrToStructure<DismPackage>(packagesPtr + i * structSize);
                result.Add(new PackageBasicInfo(
                    Marshal.PtrToStringUni(package.PackageName) ?? "",
                    package.PackageState,
                    package.ReleaseType,
                    package.InstallTime.ToDateTime()));
            }
            return result;
        }
        finally
        {
            DismApi.DismDelete(packagesPtr);
        }
    }

    /// <summary>释放 DISM 会话</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        DismApi.DismCloseSession(_session);
    }

    private static void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException("DISM API 尚未初始化，请先调用 DismSharpSession.Initialize()");
    }
}
