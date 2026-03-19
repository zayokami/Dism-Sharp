namespace DismSharp.Core.Modules;

using DismSharp.Core.Logging;
using Microsoft.Extensions.Logging;

public static class AppxManager
{
    private static readonly ILogger _logger = DismLogger.GetLogger("AppxManager");

    public static async Task<List<DismSharpSession.AppxPackageInfo>> GetPackagesAsync()
    {
        return await Task.Run(() =>
        {
            using var session = DismSharpSession.OpenOnline();
            return session.GetAppxPackages();
        }).ConfigureAwait(false);
    }

    public static async Task RemovePackageAsync(string packageName, IProgress<int>? progress = null)
    {
        await Task.Run(() =>
        {
            progress?.Report(50);
            using var session = DismSharpSession.OpenOnline();
            session.RemoveAppxPackage(packageName);
            progress?.Report(100);
            _logger.LogInformation("Removed Appx package: {PackageName}", packageName);
        }).ConfigureAwait(false);
    }
}
