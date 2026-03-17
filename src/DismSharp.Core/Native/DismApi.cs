using System.Runtime.InteropServices;

namespace DismSharp.Core.Native;

/// <summary>DismApi.dll P/Invoke 声明</summary>
internal static class DismApi
{
    private const string DismDll = "DismApi.dll";

    /// <summary>DISM 在线镜像标识</summary>
    public const string DISM_ONLINE_IMAGE = "DISM_{53BFAE52-B167-4E2F-A258-0A37B57FF845}";

    /// <summary>默认会话</summary>
    public const uint DISM_SESSION_DEFAULT = 0;

    // 挂载标志
    public const uint DISM_MOUNT_READWRITE = 0x00000000;
    public const uint DISM_MOUNT_READONLY = 0x00000001;
    public const uint DISM_MOUNT_OPTIMIZE = 0x00000002;
    public const uint DISM_MOUNT_CHECK_INTEGRITY = 0x00000004;
    public const uint DISM_MOUNT_SUPPORT_EA = 0x00000008;

    // 卸载标志
    public const uint DISM_COMMIT_IMAGE = 0x00000000;
    public const uint DISM_DISCARD_IMAGE = 0x00000001;

    // 提交标志
    public const uint DISM_COMMIT_GENERATE_INTEGRITY = 0x00010000;
    public const uint DISM_COMMIT_APPEND = 0x00020000;
    public const uint DISM_COMMIT_SUPPORT_EA = 0x00040000;
    public const uint DISM_COMMIT_MASK = 0xffff0000;

    // 预留存储状态标志
    public const uint DISM_RESERVED_STORAGE_DISABLED = 0x00000000;
    public const uint DISM_RESERVED_STORAGE_ENABLED = 0x00000001;

    /// <summary>DISM 进度回调委托</summary>
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DismProgressCallback(uint current, uint total, IntPtr userData);

    // ========== 生命周期 ==========

    /// <summary>初始化 DISM API</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismInitialize(
        DismLogLevel logLevel,
        [MarshalAs(UnmanagedType.LPWStr)] string? logFilePath,
        [MarshalAs(UnmanagedType.LPWStr)] string? scratchDirectory);

    /// <summary>关闭 DISM API</summary>
    [DllImport(DismDll)]
    public static extern int DismShutdown();

    /// <summary>释放 DISM 分配的内存</summary>
    [DllImport(DismDll)]
    public static extern int DismDelete(IntPtr dismStructure);

    // ========== 会话管理 ==========

    /// <summary>打开 DISM 会话</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismOpenSession(
        [MarshalAs(UnmanagedType.LPWStr)] string imagePath,
        [MarshalAs(UnmanagedType.LPWStr)] string? windowsDirectory,
        [MarshalAs(UnmanagedType.LPWStr)] string? systemDrive,
        out uint session);

    /// <summary>关闭 DISM 会话</summary>
    [DllImport(DismDll)]
    public static extern int DismCloseSession(uint session);

    // ========== 错误信息 ==========

    /// <summary>获取最后一次错误消息</summary>
    [DllImport(DismDll)]
    public static extern int DismGetLastErrorMessage(out IntPtr errorMessage);

    // ========== 功能管理 ==========

    /// <summary>获取所有功能列表</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismGetFeatures(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string? identifier,
        DismPackageIdentifier packageIdentifier,
        out IntPtr feature,
        out uint count);

    /// <summary>获取功能详细信息</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismGetFeatureInfo(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string featureName,
        [MarshalAs(UnmanagedType.LPWStr)] string? identifier,
        DismPackageIdentifier packageIdentifier,
        out IntPtr featureInfo);

    /// <summary>获取功能的父功能</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismGetFeatureParent(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string featureName,
        [MarshalAs(UnmanagedType.LPWStr)] string? identifier,
        DismPackageIdentifier packageIdentifier,
        out IntPtr feature,
        out uint count);

    /// <summary>启用功能</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismEnableFeature(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string featureName,
        [MarshalAs(UnmanagedType.LPWStr)] string? identifier,
        DismPackageIdentifier packageIdentifier,
        [MarshalAs(UnmanagedType.Bool)] bool limitAccess,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[]? sourcePaths,
        uint sourcePathCount,
        [MarshalAs(UnmanagedType.Bool)] bool enableAll,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>禁用功能</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismDisableFeature(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string featureName,
        [MarshalAs(UnmanagedType.LPWStr)] string? packageName,
        [MarshalAs(UnmanagedType.Bool)] bool removePayload,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    // ========== 包管理 ==========

    /// <summary>获取所有包列表</summary>
    [DllImport(DismDll)]
    public static extern int DismGetPackages(
        uint session,
        out IntPtr package,
        out uint count);

    /// <summary>获取包详细信息</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismGetPackageInfo(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string identifier,
        DismPackageIdentifier packageIdentifier,
        out IntPtr packageInfo);

    /// <summary>添加包</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismAddPackage(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string packagePath,
        [MarshalAs(UnmanagedType.Bool)] bool ignoreCheck,
        [MarshalAs(UnmanagedType.Bool)] bool preventPending,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>删除包</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismRemovePackage(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string identifier,
        DismPackageIdentifier packageIdentifier,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    // ========== 驱动管理 ==========

    /// <summary>获取驱动列表</summary>
    [DllImport(DismDll)]
    public static extern int DismGetDrivers(
        uint session,
        [MarshalAs(UnmanagedType.Bool)] bool allDrivers,
        out IntPtr driverPackage,
        out uint count);

    /// <summary>获取驱动详细信息</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismGetDriverInfo(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string driverPath,
        out IntPtr driver,
        out uint count,
        out IntPtr driverPackage);

    /// <summary>添加驱动</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismAddDriver(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string driverPath,
        [MarshalAs(UnmanagedType.Bool)] bool forceUnsigned);

    /// <summary>删除驱动</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismRemoveDriver(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string driverPath);

    // ========== 映像管理 ==========

    /// <summary>挂载映像</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismMountImage(
        [MarshalAs(UnmanagedType.LPWStr)] string imageFilePath,
        [MarshalAs(UnmanagedType.LPWStr)] string mountPath,
        uint imageIndex,
        [MarshalAs(UnmanagedType.LPWStr)] string? imageName,
        DismImageIdentifier imageIdentifier,
        uint flags,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>卸载映像</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismUnmountImage(
        [MarshalAs(UnmanagedType.LPWStr)] string mountPath,
        uint flags,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>重新挂载映像</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismRemountImage(
        [MarshalAs(UnmanagedType.LPWStr)] string mountPath);

    /// <summary>提交映像更改</summary>
    [DllImport(DismDll)]
    public static extern int DismCommitImage(
        uint session,
        uint flags,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>获取映像信息</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismGetImageInfo(
        [MarshalAs(UnmanagedType.LPWStr)] string imageFilePath,
        out IntPtr imageInfo,
        out uint count);

    /// <summary>获取已挂载映像信息</summary>
    [DllImport(DismDll)]
    public static extern int DismGetMountedImageInfo(
        out IntPtr mountedImageInfo,
        out uint count);

    /// <summary>清理挂载点</summary>
    [DllImport(DismDll)]
    public static extern int DismCleanupMountpoints();

    /// <summary>检查映像健康状态</summary>
    [DllImport(DismDll)]
    public static extern int DismCheckImageHealth(
        uint session,
        [MarshalAs(UnmanagedType.Bool)] bool scanImage,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData,
        out DismImageHealthState imageHealth);

    /// <summary>修复映像</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismRestoreImageHealth(
        uint session,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[]? sourcePaths,
        uint sourcePathCount,
        [MarshalAs(UnmanagedType.Bool)] bool limitAccess,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>应用无人值守文件</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismApplyUnattend(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string unattendFile,
        [MarshalAs(UnmanagedType.Bool)] bool singleSession);

    // ========== 能力管理 ==========

    /// <summary>获取能力列表</summary>
    [DllImport(DismDll)]
    public static extern int DismGetCapabilities(
        uint session,
        out IntPtr capability,
        out uint count);

    /// <summary>获取能力详细信息</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismGetCapabilityInfo(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string name,
        out IntPtr info);

    /// <summary>添加能力</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismAddCapability(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string name,
        [MarshalAs(UnmanagedType.Bool)] bool limitAccess,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[]? sourcePaths,
        uint sourcePathCount,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>删除能力</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismRemoveCapability(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string name,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    // ========== 预留存储 ==========

    /// <summary>获取预留存储状态</summary>
    [DllImport(DismDll)]
    public static extern int DismGetReservedStorageState(
        uint session,
        out uint state);

    /// <summary>设置预留存储状态</summary>
    [DllImport(DismDll)]
    public static extern int DismSetReservedStorageState(
        uint session,
        uint state);

    // ========== Appx 管理 ==========

    /// <summary>获取预配置 Appx 包列表</summary>
    [DllImport(DismDll, EntryPoint = "_DismGetProvisionedAppxPackages")]
    public static extern int DismGetProvisionedAppxPackages(
        uint session,
        out IntPtr package,
        out uint count);

    /// <summary>添加预配置 Appx 包</summary>
    [DllImport(DismDll, EntryPoint = "_DismAddProvisionedAppxPackage", CharSet = CharSet.Unicode)]
    public static extern int DismAddProvisionedAppxPackage(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string appPath,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[]? dependencyPackages,
        uint dependencyPackageCount,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[]? optionalPackages,
        uint optionalPackageCount,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[]? licensePaths,
        uint licensePathCount,
        [MarshalAs(UnmanagedType.Bool)] bool skipLicense,
        [MarshalAs(UnmanagedType.LPWStr)] string? customDataPath,
        [MarshalAs(UnmanagedType.LPWStr)] string? region,
        DismStubPackageOption stubPackageOption);

    /// <summary>删除预配置 Appx 包</summary>
    [DllImport(DismDll, EntryPoint = "_DismRemoveProvisionedAppxPackage", CharSet = CharSet.Unicode)]
    public static extern int DismRemoveProvisionedAppxPackage(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string packageName);

    // ========== 语言管理 ==========

    /// <summary>添加语言</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismAddLanguage(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string languageName,
        [MarshalAs(UnmanagedType.Bool)] bool preventPending,
        [MarshalAs(UnmanagedType.Bool)] bool limitAccess,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] sourcePaths,
        uint sourcePathCount,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);

    /// <summary>删除语言</summary>
    [DllImport(DismDll, CharSet = CharSet.Unicode)]
    public static extern int DismRemoveLanguage(
        uint session,
        [MarshalAs(UnmanagedType.LPWStr)] string languageName,
        IntPtr cancelEvent,
        DismProgressCallback? progress,
        IntPtr userData);
}
