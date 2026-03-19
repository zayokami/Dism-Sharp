using DismSharp.Core.CleanupRules;
using DismSharp.Core.Logging;
using Microsoft.Extensions.Logging;

namespace DismSharp.Core.Modules;

/// <summary>清理规则扫描结果</summary>
public record CleanupScanResult(
    ICleanupRule Rule,
    List<CleanupEntry> Entries,
    long TotalBytes)
{
    public bool IsSelected { get; set; } = true;
}

/// <summary>系统清理引擎，协调扫描和清理操作</summary>
public static class CleanupEngine
{
    /// <summary>获取所有内置清理规则</summary>
    public static List<ICleanupRule> GetDefaultRules() =>
    [
        new TempFilesRule(),
        new WindowsUpdateCacheRule(),
        new LogFilesRule(),
        new PrefetchRule(),
        new RecycleBinRule(),
        new ThumbnailCacheRule(),
        new WindowsOldRule(),
        new HibernateFileRule(),
        new DeliveryOptimizationRule(),
        new CrashDumpsRule(),
        new ErrorReportsRule(),
        new DirectXShaderCacheRule(),
    ];

    /// <summary>扫描所有规则，返回各规则的可清理项</summary>
    public static async Task<List<CleanupScanResult>> ScanAsync(
        IEnumerable<ICleanupRule> rules,
        IProgress<(string ruleName, int ruleIndex, int totalRules)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var ruleList = rules.ToList();
        var results = new List<CleanupScanResult>();

        for (int i = 0; i < ruleList.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rule = ruleList[i];
            progress?.Report((rule.Name, i, ruleList.Count));

            try
            {
                var entries = await rule.ScanAsync(cancellationToken).ConfigureAwait(false);
                long totalBytes = entries.Sum(e => e.SizeBytes);
                results.Add(new CleanupScanResult(rule, entries, totalBytes));
            }
            catch (Exception ex)
            {
                DismLogger.GetLogger("CleanupEngine").LogError(ex, "Scan failed");
                results.Add(new CleanupScanResult(rule, [], 0));
            }
        }

        return results;
    }

    /// <summary>执行清理操作</summary>
    /// <param name="scanResults">扫描结果（仅处理 IsSelected 为 true 的）</param>
    /// <param name="progress">进度回调（规则名, 规则索引, 总规则数, 清理的文件数）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总清理字节数</returns>
    public static async Task<long> CleanAsync(
        List<CleanupScanResult> scanResults,
        IProgress<(string ruleName, int ruleIndex, int totalRules, int cleanedFiles)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var selectedResults = scanResults.Where(r => r.IsSelected && r.Entries.Count > 0).ToList();
        long totalCleanedBytes = 0;

        for (int i = 0; i < selectedResults.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = selectedResults[i];
            progress?.Report((result.Rule.Name, i, selectedResults.Count, 0));

            try
            {
                int cleaned = await result.Rule.CleanAsync(result.Entries, null, cancellationToken).ConfigureAwait(false);
                totalCleanedBytes += result.TotalBytes; // 近似值
                progress?.Report((result.Rule.Name, i, selectedResults.Count, cleaned));
            }
            catch (Exception ex) { DismLogger.GetLogger("CleanupEngine").LogError(ex, "Clean failed"); }
        }

        return totalCleanedBytes;
    }

    /// <summary>格式化字节数为可读字符串</summary>
    public static string FormatBytes(long bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        int i = 0;
        double dblBytes = bytes;

        while (dblBytes >= 1024 && i < suffixes.Length - 1)
        {
            dblBytes /= 1024;
            i++;
        }

        return $"{dblBytes:F1} {suffixes[i]}";
    }
}
