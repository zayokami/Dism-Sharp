using DismSharp.Core.Native;

namespace DismSharp.Core.Modules;

/// <summary>Windows 功能管理器，提供异步启用/禁用功能操作</summary>
public static class FeatureManager
{
    /// <summary>异步启用 Windows 功能</summary>
    /// <param name="session">DISM 会话</param>
    /// <param name="featureName">功能名称</param>
    /// <param name="enableAll">是否同时启用所有父功能</param>
    /// <param name="progress">进度回调（0-100）</param>
    /// <param name="cancellationToken">取消令牌</param>
    public static async Task EnableFeatureAsync(
        DismSharpSession session,
        string featureName,
        bool enableAll = true,
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

            int hr = DismApi.DismEnableFeature(
                handle,
                featureName,
                null,
                DismPackageIdentifier.DismPackageNone,
                false,
                null,
                0,
                enableAll,
                IntPtr.Zero,
                callback,
                IntPtr.Zero);

            DismSharpException.ThrowIfFailed(hr);
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>异步禁用 Windows 功能</summary>
    /// <param name="session">DISM 会话</param>
    /// <param name="featureName">功能名称</param>
    /// <param name="removePayload">是否移除功能负载</param>
    /// <param name="progress">进度回调（0-100）</param>
    /// <param name="cancellationToken">取消令牌</param>
    public static async Task DisableFeatureAsync(
        DismSharpSession session,
        string featureName,
        bool removePayload = false,
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

            int hr = DismApi.DismDisableFeature(
                handle,
                featureName,
                null,
                removePayload,
                IntPtr.Zero,
                callback,
                IntPtr.Zero);

            DismSharpException.ThrowIfFailed(hr);
        }, cancellationToken).ConfigureAwait(false);
    }
}
