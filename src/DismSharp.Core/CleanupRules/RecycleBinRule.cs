using DismSharp.Core.Native;

namespace DismSharp.Core.CleanupRules;

/// <summary>回收站清理规则</summary>
public class RecycleBinRule : ICleanupRule
{
    public string Name => "回收站";
    public string Description => "清空所有磁盘上的回收站";

    public Task<List<CleanupEntry>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var entries = new List<CleanupEntry>();

            try
            {
                var info = new Win32Api.SHQUERYRBINFO
                {
                    cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<Win32Api.SHQUERYRBINFO>()
                };

                int hr = Win32Api.SHQueryRecycleBin(null, ref info);
                if (hr == 0 && info.i64Size > 0)
                {
                    // 回收站作为一个整体条目
                    entries.Add(new CleanupEntry("$RECYCLE.BIN", info.i64Size));
                }
            }
            catch
            {
                // 查询失败
            }

            return entries;
        }, cancellationToken);
    }

    public Task<int> CleanAsync(
        List<CleanupEntry> entries,
        IProgress<(int current, int total)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (entries.Count == 0) return 0;

            try
            {
                int hr = Win32Api.SHEmptyRecycleBin(
                    IntPtr.Zero, null,
                    Win32Api.SHERB_NOCONFIRMATION | Win32Api.SHERB_NOPROGRESSUI | Win32Api.SHERB_NOSOUND);

                progress?.Report((1, 1));

                // S_OK = 0, S_FALSE when already empty
                return (hr == 0 || hr == 1) ? 1 : 0;
            }
            catch
            {
                return 0;
            }
        }, cancellationToken);
    }
}
