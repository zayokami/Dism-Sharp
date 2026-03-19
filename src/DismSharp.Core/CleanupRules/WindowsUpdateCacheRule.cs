using System.Diagnostics;
using DismSharp.Core.Helpers;

namespace DismSharp.Core.CleanupRules;

/// <summary>Windows Update 缓存清理规则</summary>
public class WindowsUpdateCacheRule : FileCleanupRule
{
    public override string Name => "Windows Update 缓存";
    public override string Description => "清理 Windows 更新下载缓存（SoftwareDistribution\\Download）";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var downloadPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "SoftwareDistribution", "Download");

            return ScanDirectory(downloadPath);
        }, cancellationToken);
    }

    public override async Task<int> CleanAsync(
        List<CleanupEntry> entries,
        IProgress<(int current, int total)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // 清理前需停止 Windows Update 服务
        bool wasRunning = await Task.Run(() => ServiceHelper.StopService("wuauserv"), cancellationToken).ConfigureAwait(false);

        try
        {
            return await base.CleanAsync(entries, progress, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // 清理完毕后恢复服务
            if (wasRunning)
                await Task.Run(() => ServiceHelper.StartService("wuauserv"), cancellationToken).ConfigureAwait(false);
        }
    }
}
