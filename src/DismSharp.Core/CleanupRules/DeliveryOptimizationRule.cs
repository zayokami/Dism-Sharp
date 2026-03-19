namespace DismSharp.Core.CleanupRules;

public class DeliveryOptimizationRule : FileCleanupRule
{
    public override string Name => "Delivery Optimization";
    public override string Description => "清理 Windows 更新 P2P 分发缓存";

    public override Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var entries = new List<CleanupEntry>();

            var cachePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "ServiceProfiles", "NetworkService",
                "AppData", "Local", "Microsoft", "Windows", "DeliveryOptimization", "Cache");

            if (Directory.Exists(cachePath))
                entries.AddRange(ScanDirectory(cachePath));

            var altPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "SoftwareDistribution", "DeliveryOptimization", "Cache");

            if (Directory.Exists(altPath) && altPath != cachePath)
                entries.AddRange(ScanDirectory(altPath));

            return entries;
        }, cancellationToken);
    }
}
