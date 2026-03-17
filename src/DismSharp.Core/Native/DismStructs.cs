using System.Runtime.InteropServices;

namespace DismSharp.Core.Native;

/// <summary>Windows SYSTEMTIME 结构体</summary>
[StructLayout(LayoutKind.Sequential)]
public struct SystemTime
{
    public ushort Year;
    public ushort Month;
    public ushort DayOfWeek;
    public ushort Day;
    public ushort Hour;
    public ushort Minute;
    public ushort Second;
    public ushort Milliseconds;

    /// <summary>转换为 DateTime</summary>
    public readonly DateTime ToDateTime()
    {
        if (Year == 0) return DateTime.MinValue;
        return new DateTime(Year, Month, Day, Hour, Minute, Second, Milliseconds);
    }
}

/// <summary>DISM 包信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismPackage
{
    public IntPtr PackageName;
    public DismPackageFeatureState PackageState;
    public DismReleaseType ReleaseType;
    public SystemTime InstallTime;
}

/// <summary>DISM 自定义属性</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismCustomProperty
{
    public IntPtr Name;
    public IntPtr Value;
    public IntPtr Path;
}

/// <summary>DISM 功能信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismFeature
{
    public IntPtr FeatureName;
    public DismPackageFeatureState State;
}

/// <summary>DISM 能力信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismCapability
{
    public IntPtr Name;
    public DismPackageFeatureState State;
}

/// <summary>DISM 包详细信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismPackageInfoNative
{
    public IntPtr PackageName;
    public DismPackageFeatureState PackageState;
    public DismReleaseType ReleaseType;
    public SystemTime InstallTime;
    public int Applicable;
    public IntPtr Copyright;
    public IntPtr Company;
    public SystemTime CreationTime;
    public IntPtr DisplayName;
    public IntPtr Description;
    public IntPtr InstallClient;
    public IntPtr InstallPackageName;
    public SystemTime LastUpdateTime;
    public IntPtr ProductName;
    public IntPtr ProductVersion;
    public DismRestartType RestartRequired;
    public DismFullyOfflineInstallableType FullyOffline;
    public IntPtr SupportInformation;
    public IntPtr CustomProperty;
    public uint CustomPropertyCount;
    public IntPtr Feature;
    public uint FeatureCount;
}

/// <summary>DISM 功能详细信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismFeatureInfoNative
{
    public IntPtr FeatureName;
    public DismPackageFeatureState FeatureState;
    public IntPtr DisplayName;
    public IntPtr Description;
    public DismRestartType RestartRequired;
    public IntPtr CustomProperty;
    public uint CustomPropertyCount;
}

/// <summary>DISM 能力详细信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismCapabilityInfoNative
{
    public IntPtr Name;
    public DismPackageFeatureState State;
    public IntPtr DisplayName;
    public IntPtr Description;
    public uint DownloadSize;
    public uint InstallSize;
}

/// <summary>DISM 字符串</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismString
{
    public IntPtr Value;
}

/// <summary>DISM WIM 自定义信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismWimCustomizedInfo
{
    public uint Size;
    public uint DirectoryCount;
    public uint FileCount;
    public SystemTime CreatedTime;
    public SystemTime ModifiedTime;
}

/// <summary>DISM 映像信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismImageInfo
{
    public DismImageType ImageType;
    public uint ImageIndex;
    public IntPtr ImageName;
    public IntPtr ImageDescription;
    public ulong ImageSize;
    public uint Architecture;
    public IntPtr ProductName;
    public IntPtr EditionId;
    public IntPtr InstallationType;
    public IntPtr Hal;
    public IntPtr ProductType;
    public IntPtr ProductSuite;
    public uint MajorVersion;
    public uint MinorVersion;
    public uint Build;
    public uint SpBuild;
    public uint SpLevel;
    public DismImageBootable Bootable;
    public IntPtr SystemRoot;
    public IntPtr Language;
    public uint LanguageCount;
    public uint DefaultLanguageIndex;
    public IntPtr CustomizedInfo;
}

/// <summary>DISM 已挂载映像信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismMountedImageInfo
{
    public IntPtr MountPath;
    public IntPtr ImageFilePath;
    public uint ImageIndex;
    public DismMountMode MountMode;
    public DismMountStatus MountStatus;
}

/// <summary>DISM 驱动包信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismDriverPackage
{
    public IntPtr PublishedName;
    public IntPtr OriginalFileName;
    public int InBox;
    public IntPtr CatalogFile;
    public IntPtr ClassName;
    public IntPtr ClassGuid;
    public IntPtr ClassDescription;
    public int BootCritical;
    public DismDriverSignature DriverSignature;
    public IntPtr ProviderName;
    public SystemTime Date;
    public uint MajorVersion;
    public uint MinorVersion;
    public uint Build;
    public uint Revision;

    /// <summary>是否为系统内置驱动</summary>
    public readonly bool IsInBox => InBox != 0;

    /// <summary>是否为启动关键驱动</summary>
    public readonly bool IsBootCritical => BootCritical != 0;
}

/// <summary>DISM 驱动信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismDriver
{
    public IntPtr ManufacturerName;
    public IntPtr HardwareDescription;
    public IntPtr HardwareId;
    public uint Architecture;
    public IntPtr ServiceName;
    public IntPtr CompatibleIds;
    public IntPtr ExcludeIds;
}

/// <summary>DISM Appx 包信息</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
public struct DismAppxPackage
{
    public IntPtr PackageName;
    public IntPtr DisplayName;
    public IntPtr PublisherId;
    public uint MajorVersion;
    public uint MinorVersion;
    public uint Build;
    public uint RevisionNumber;
    public uint Architecture;
    public IntPtr ResourceId;
    public IntPtr InstallLocation;
    public IntPtr Region;
}
