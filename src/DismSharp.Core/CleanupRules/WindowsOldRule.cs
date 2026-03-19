namespace DismSharp.Core.CleanupRules;

public class WindowsOldRule : FileCleanupRule
{
    public override string Name => "Windows.old";
    public override string Description => "清理旧版 Windows 备份文件夹（升级后残留）";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var windowsOld = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "..", "Windows.old");

            var fullPath = Path.GetFullPath(windowsOld);
            return Directory.Exists(fullPath) ? ScanDirectory(fullPath) : [];
        }, cancellationToken);
    }
}
