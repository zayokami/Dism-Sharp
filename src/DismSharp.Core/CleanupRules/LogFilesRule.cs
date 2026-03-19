namespace DismSharp.Core.CleanupRules;

using System.Diagnostics;

/// <summary>系统日志文件清理规则</summary>
public class LogFilesRule : FileCleanupRule
{
    public override string Name => "系统日志";
    public override string Description => "清理 Windows 日志目录中的日志文件（.log, .etl）";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var entries = new List<CleanupEntry>();
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            // Windows\Logs 目录
            entries.AddRange(ScanDirectory(Path.Combine(winDir, "Logs"), "*.log"));
            entries.AddRange(ScanDirectory(Path.Combine(winDir, "Logs"), "*.etl"));

            // Windows\Panther 目录（安装日志）
            entries.AddRange(ScanDirectory(Path.Combine(winDir, "Panther"), "*.log"));

            // Windows\SoftwareDistribution\ReportingEvents.log
            var reportLog = Path.Combine(winDir, "SoftwareDistribution", "ReportingEvents.log");
            if (File.Exists(reportLog))
            {
                try
                {
                    var info = new FileInfo(reportLog);
                    entries.Add(new CleanupEntry(reportLog, info.Length));
                }
                catch (Exception ex) { Debug.WriteLine($"[{GetType().Name}] Error: {ex.Message}"); }
            }

            return entries;
        }, cancellationToken);
    }
}
