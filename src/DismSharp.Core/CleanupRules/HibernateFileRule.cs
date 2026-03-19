using System.Diagnostics;

namespace DismSharp.Core.CleanupRules;

public class HibernateFileRule : FileCleanupRule
{
    public override string Name => "休眠文件";
    public override string Description => "禁用休眠并删除 hiberfil.sys（可释放数 GB 空间）";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var hiberfil = Path.Combine(
                Path.GetPathRoot(Environment.SystemDirectory) ?? @"C:\",
                "hiberfil.sys");

            if (File.Exists(hiberfil))
            {
                var size = new FileInfo(hiberfil).Length;
                return new List<CleanupEntry> { new CleanupEntry(hiberfil, size) };
            }
            return [];
        }, cancellationToken);
    }

    public override async Task<int> CleanAsync(
        List<CleanupEntry> entries,
        IProgress<(int current, int total)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (entries.Count == 0) return 0;

        await Task.Run(() =>
        {
            progress?.Report((0, 1));

            var psi = new ProcessStartInfo("powercfg", "/hibernate off")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Verb = "runas"
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();

            progress?.Report((1, 1));
        }, cancellationToken).ConfigureAwait(false);

        return entries.Count;
    }
}
