namespace DismSharp.Core.CleanupRules;

/// <summary>临时文件清理规则</summary>
public class TempFilesRule : FileCleanupRule
{
    public override string Name => "临时文件";
    public override string Description => "清理用户和系统临时目录中的临时文件";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var entries = new List<CleanupEntry>();

            // 用户临时目录
            var userTemp = Path.GetTempPath();
            entries.AddRange(ScanDirectory(userTemp));

            // 系统临时目录
            var winTemp = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
            if (winTemp != userTemp)
                entries.AddRange(ScanDirectory(winTemp));

            return entries;
        }, cancellationToken);
    }
}
