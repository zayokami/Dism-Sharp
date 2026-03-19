namespace DismSharp.Core.CleanupRules;

public class ErrorReportsRule : FileCleanupRule
{
    public override string Name => "Windows 错误报告";
    public override string Description => "清理 Windows 错误报告文件（WERReportArchive、ReportQueue）";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var entries = new List<CleanupEntry>();

            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            var paths = new[]
            {
                Path.Combine(programData, "Microsoft", "Windows", "WER", "ReportArchive"),
                Path.Combine(programData, "Microsoft", "Windows", "WER", "ReportQueue"),
                Path.Combine(programData, "Microsoft", "Windows", "WER", "Temp"),
            };

            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                    entries.AddRange(ScanDirectory(path));
            }

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localWer = Path.Combine(localAppData, "Microsoft", "Windows", "WER");

            if (Directory.Exists(localWer))
                entries.AddRange(ScanDirectory(localWer));

            return entries;
        }, cancellationToken);
    }
}
