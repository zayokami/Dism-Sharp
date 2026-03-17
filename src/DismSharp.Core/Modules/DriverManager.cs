using DismSharp.Core.Native;

namespace DismSharp.Core.Modules;

/// <summary>驱动管理器，提供驱动备份和还原操作</summary>
public static class DriverManager
{
    /// <summary>备份第三方驱动到指定目录</summary>
    /// <param name="session">DISM 会话</param>
    /// <param name="backupDirectory">备份目录路径</param>
    /// <param name="progress">进度回调（0-100）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>成功备份的驱动数量</returns>
    public static async Task<int> BackupDriversAsync(
        DismSharpSession session,
        string backupDirectory,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var drivers = session.GetDrivers(allDrivers: false);
            if (drivers.Count == 0) return 0;

            Directory.CreateDirectory(backupDirectory);

            int completed = 0;
            foreach (var driver in drivers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    // 驱动 inf 文件位于 DriverStore 中
                    var infPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                        "System32", "DriverStore", "FileRepository");

                    // 查找与驱动发布名匹配的目录
                    var publishedName = Path.GetFileNameWithoutExtension(driver.PublishedName);
                    var driverDirs = Directory.GetDirectories(infPath, $"{publishedName}*");

                    foreach (var driverDir in driverDirs)
                    {
                        var destDir = Path.Combine(backupDirectory, Path.GetFileName(driverDir));
                        CopyDirectory(driverDir, destDir);
                    }

                    completed++;
                }
                catch
                {
                    // 单个驱动备份失败不影响其他驱动
                }

                progress?.Report((int)((double)completed / drivers.Count * 100));
            }

            return completed;
        }, cancellationToken);
    }

    /// <summary>从备份目录还原驱动</summary>
    /// <param name="session">DISM 会话</param>
    /// <param name="driverPath">驱动 .inf 文件路径</param>
    /// <param name="forceUnsigned">是否强制安装未签名驱动</param>
    public static async Task AddDriverAsync(
        DismSharpSession session,
        string driverPath,
        bool forceUnsigned = false)
    {
        var handle = session.SessionHandle;

        await Task.Run(() =>
        {
            int hr = DismApi.DismAddDriver(handle, driverPath, forceUnsigned);
            DismSharpException.ThrowIfFailed(hr);
        });
    }

    /// <summary>删除驱动</summary>
    /// <param name="session">DISM 会话</param>
    /// <param name="driverPublishedName">驱动发布名（如 oem0.inf）</param>
    public static async Task RemoveDriverAsync(
        DismSharpSession session,
        string driverPublishedName)
    {
        var handle = session.SessionHandle;

        await Task.Run(() =>
        {
            int hr = DismApi.DismRemoveDriver(handle, driverPublishedName);
            DismSharpException.ThrowIfFailed(hr);
        });
    }

    /// <summary>扫描备份目录，列出所有可还原的 .inf 文件</summary>
    /// <param name="backupDirectory">备份目录</param>
    /// <returns>.inf 文件路径列表</returns>
    public static List<string> ScanBackupDirectory(string backupDirectory)
    {
        if (!Directory.Exists(backupDirectory))
            return [];

        return Directory.GetFiles(backupDirectory, "*.inf", SearchOption.AllDirectories).ToList();
    }

    /// <summary>递归复制目录</summary>
    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), overwrite: true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            CopyDirectory(dir, Path.Combine(destDir, Path.GetFileName(dir)));
        }
    }
}
