namespace DismSharp.Core.CleanupRules;

/// <summary>缩略图缓存清理规则</summary>
public class ThumbnailCacheRule : FileCleanupRule
{
    public override string Name => "缩略图缓存";
    public override string Description => "清理 Windows 资源管理器的缩略图缓存文件";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var explorerCache = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft", "Windows", "Explorer");

            return ScanDirectory(explorerCache, "thumbcache_*", SearchOption.TopDirectoryOnly);
        }, cancellationToken);
    }
}
