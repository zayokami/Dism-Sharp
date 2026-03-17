using System.Runtime.InteropServices;

namespace DismSharp.Core.Native;

/// <summary>DISM 日志级别</summary>
public enum DismLogLevel
{
    /// <summary>仅记录错误</summary>
    DismLogErrors = 0,
    /// <summary>记录错误和警告</summary>
    DismLogErrorsWarnings,
    /// <summary>记录错误、警告和信息</summary>
    DismLogErrorsWarningsInfo,
    /// <summary>记录错误、警告、信息和调试</summary>
    DismLogErrorsWarningsInfoDebug
}

/// <summary>DISM 映像标识类型</summary>
public enum DismImageIdentifier
{
    /// <summary>按索引标识</summary>
    DismImageIndex = 0,
    /// <summary>按名称标识</summary>
    DismImageName,
    /// <summary>无标识</summary>
    DismImageNone
}

/// <summary>DISM 挂载模式</summary>
public enum DismMountMode
{
    /// <summary>读写模式</summary>
    DismReadWrite = 0,
    /// <summary>只读模式</summary>
    DismReadOnly
}

/// <summary>DISM 映像类型</summary>
public enum DismImageType
{
    /// <summary>不支持的映像类型</summary>
    DismImageTypeUnsupported = -1,
    /// <summary>WIM 映像</summary>
    DismImageTypeWim = 0,
    /// <summary>VHD 映像</summary>
    DismImageTypeVhd = 1
}

/// <summary>DISM 映像可启动状态</summary>
public enum DismImageBootable
{
    /// <summary>可启动</summary>
    DismImageBootableYes = 0,
    /// <summary>不可启动</summary>
    DismImageBootableNo,
    /// <summary>未知</summary>
    DismImageBootableUnknown
}

/// <summary>DISM 挂载状态</summary>
public enum DismMountStatus
{
    /// <summary>正常</summary>
    DismMountStatusOk = 0,
    /// <summary>需要重新挂载</summary>
    DismMountStatusNeedsRemount,
    /// <summary>无效</summary>
    DismMountStatusInvalid
}

/// <summary>DISM 映像健康状态</summary>
public enum DismImageHealthState
{
    /// <summary>健康</summary>
    DismImageHealthy = 0,
    /// <summary>可修复</summary>
    DismImageRepairable,
    /// <summary>不可修复</summary>
    DismImageNonRepairable
}

/// <summary>DISM 包标识类型</summary>
public enum DismPackageIdentifier
{
    /// <summary>无标识</summary>
    DismPackageNone = 0,
    /// <summary>按名称标识</summary>
    DismPackageName,
    /// <summary>按路径标识</summary>
    DismPackagePath
}

/// <summary>DISM 包/功能状态</summary>
public enum DismPackageFeatureState
{
    /// <summary>不存在</summary>
    DismStateNotPresent = 0,
    /// <summary>卸载挂起</summary>
    DismStateUninstallPending,
    /// <summary>已暂存</summary>
    DismStateStaged,
    /// <summary>内部使用</summary>
    DismStateResolved,
    /// <summary>已移除（等同于 DismStateResolved）</summary>
    DismStateRemoved = DismStateResolved,
    /// <summary>已安装</summary>
    DismStateInstalled,
    /// <summary>安装挂起</summary>
    DismStateInstallPending,
    /// <summary>已被取代</summary>
    DismStateSuperseded,
    /// <summary>部分安装</summary>
    DismStatePartiallyInstalled
}

/// <summary>DISM 发布类型</summary>
public enum DismReleaseType
{
    /// <summary>关键更新</summary>
    DismReleaseTypeCriticalUpdate = 0,
    /// <summary>驱动</summary>
    DismReleaseTypeDriver,
    /// <summary>功能包</summary>
    DismReleaseTypeFeaturePack,
    /// <summary>热修复</summary>
    DismReleaseTypeHotfix,
    /// <summary>安全更新</summary>
    DismReleaseTypeSecurityUpdate,
    /// <summary>软件更新</summary>
    DismReleaseTypeSoftwareUpdate,
    /// <summary>更新</summary>
    DismReleaseTypeUpdate,
    /// <summary>更新汇总</summary>
    DismReleaseTypeUpdateRollup,
    /// <summary>语言包</summary>
    DismReleaseTypeLanguagePack,
    /// <summary>基础</summary>
    DismReleaseTypeFoundation,
    /// <summary>服务包</summary>
    DismReleaseTypeServicePack,
    /// <summary>产品</summary>
    DismReleaseTypeProduct,
    /// <summary>本地包</summary>
    DismReleaseTypeLocalPack,
    /// <summary>其他</summary>
    DismReleaseTypeOther,
    /// <summary>按需功能包</summary>
    DismReleaseTypeOnDemandPack
}

/// <summary>DISM 重启类型</summary>
public enum DismRestartType
{
    /// <summary>不需要重启</summary>
    DismRestartNo = 0,
    /// <summary>可能需要重启</summary>
    DismRestartPossible,
    /// <summary>需要重启</summary>
    DismRestartRequired
}

/// <summary>DISM 驱动签名状态</summary>
public enum DismDriverSignature
{
    /// <summary>未知</summary>
    DismDriverSignatureUnknown = 0,
    /// <summary>未签名</summary>
    DismDriverSignatureUnsigned = 1,
    /// <summary>已签名</summary>
    DismDriverSignatureSigned = 2
}

/// <summary>DISM 完全离线可安装类型</summary>
public enum DismFullyOfflineInstallableType
{
    /// <summary>可离线安装</summary>
    DismFullyOfflineInstallable = 0,
    /// <summary>不可离线安装</summary>
    DismFullyOfflineNotInstallable,
    /// <summary>无法确定</summary>
    DismFullyOfflineInstallableUndetermined
}

/// <summary>DISM Stub 包选项</summary>
public enum DismStubPackageOption
{
    /// <summary>无</summary>
    DismStubPackageOptionNone = 0,
    /// <summary>安装完整版</summary>
    DismStubPackageOptionInstallFull = 1,
    /// <summary>安装精简版</summary>
    DismStubPackageOptionInstallStub = 2
}
