namespace DismSharp.Core.CleanupRules;

/// <summary>预读取文件清理规则</summary>
public class PrefetchRule : FileCleanupRule
{
    public override string Name => "预读取缓存";
    public override string Description => "清理 Windows 预读取文件（Prefetch）";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var prefetchPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");

            return ScanDirectory(prefetchPath, "*.pf", SearchOption.TopDirectoryOnly);
        }, cancellationToken);
    }
}
