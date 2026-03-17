namespace DismSharp.Core.CleanupRules;

/// <summary>文件清理规则基类，提供通用的扫描和删除逻辑</summary>
public abstract class FileCleanupRule : ICleanupRule
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default);

    public virtual async Task<int> CleanAsync(
        List<CleanupEntry> entries,
        IProgress<(int current, int total)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            int cleaned = 0;
            int total = entries.Count;

            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    if (File.Exists(entry.FilePath))
                    {
                        File.Delete(entry.FilePath);
                        cleaned++;
                    }
                    else if (Directory.Exists(entry.FilePath))
                    {
                        Directory.Delete(entry.FilePath, recursive: true);
                        cleaned++;
                    }
                }
                catch
                {
                    // 单个文件删除失败不影响整体
                }

                progress?.Report((cleaned, total));
            }

            return cleaned;
        }, cancellationToken);
    }

    /// <summary>扫描指定目录中的文件</summary>
    protected static List<CleanupEntry> ScanDirectory(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
    {
        var entries = new List<CleanupEntry>();

        if (!Directory.Exists(path))
            return entries;

        try
        {
            var files = Directory.EnumerateFiles(path, searchPattern, new EnumerationOptions
            {
                RecurseSubdirectories = searchOption == SearchOption.AllDirectories,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.System
            });

            foreach (var file in files)
            {
                try
                {
                    var info = new FileInfo(file);
                    entries.Add(new CleanupEntry(file, info.Length));
                }
                catch
                {
                    // 无法访问的文件跳过
                }
            }
        }
        catch
        {
            // 目录无法访问
        }

        return entries;
    }
}
