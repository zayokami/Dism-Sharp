namespace DismSharp.Core.CleanupRules;

public class CrashDumpsRule : FileCleanupRule
{
    public override string Name => "崩溃转储";
    public override string Description => "清理应用程序崩溃转储文件（MEMORY.DMP、WER 报告）";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var entries = new List<CleanupEntry>();

            var minidump = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "Minidump");

            if (Directory.Exists(minidump))
                entries.AddRange(ScanDirectory(minidump));

            var memoryDump = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "MEMORY.DMP");

            if (File.Exists(memoryDump))
            {
                var size = new FileInfo(memoryDump).Length;
                entries.Add(new CleanupEntry(memoryDump, size));
            }

            var localDumps = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CrashDumps");

            if (Directory.Exists(localDumps))
                entries.AddRange(ScanDirectory(localDumps));

            return entries;
        }, cancellationToken);
    }
}
