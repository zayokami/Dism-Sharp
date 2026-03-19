using DismSharp.Core.Native;

namespace DismSharp.Core.Modules;

/// <summary>更新包管理器，提供异步卸载更新包操作</summary>
public static class PackageManager
{
    /// <summary>异步卸载更新包</summary>
    /// <param name="session">DISM 会话</param>
    /// <param name="packageName">包名称</param>
    /// <param name="progress">进度回调（0-100）</param>
    /// <param name="cancellationToken">取消令牌</param>
    public static async Task UninstallPackageAsync(
        DismSharpSession session,
        string packageName,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var handle = session.SessionHandle;

        await Task.Run(() =>
        {
            DismApi.DismProgressCallback? callback = null;
            if (progress is not null)
            {
                callback = (current, total, _) =>
                {
                    if (total > 0)
                        progress.Report((int)(current * 100 / total));
                };
            }

            int hr = DismApi.DismRemovePackage(
                handle,
                packageName,
                DismPackageIdentifier.DismPackageName,
                IntPtr.Zero,
                callback,
                IntPtr.Zero);

            DismSharpException.ThrowIfFailed(hr);
        }, cancellationToken).ConfigureAwait(false);
    }
}
