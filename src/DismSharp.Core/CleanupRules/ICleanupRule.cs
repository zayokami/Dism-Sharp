namespace DismSharp.Core.CleanupRules;

/// <summary>清理项条目</summary>
/// <param name="FilePath">文件完整路径</param>
/// <param name="SizeBytes">文件大小（字节）</param>
public record CleanupEntry(string FilePath, long SizeBytes);

/// <summary>清理规则接口</summary>
public interface ICleanupRule
{
    /// <summary>规则名称</summary>
    string Name { get; }

    /// <summary>规则描述</summary>
    string Description { get; }

    /// <summary>扫描可清理的文件</summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>清理项列表</returns>
    Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default);

    /// <summary>执行清理</summary>
    /// <param name="entries">要清理的项</param>
    /// <param name="progress">进度回调（已清理文件数, 总文件数）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>成功清理的文件数量</returns>
    Task<int> CleanAsync(
        List<CleanupEntry> entries,
        IProgress<(int current, int total)>? progress = null,
        CancellationToken cancellationToken = default);
}
