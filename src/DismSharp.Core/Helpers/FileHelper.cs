namespace DismSharp.Core.Helpers;

/// <summary>文件操作辅助工具</summary>
public static class FileHelper
{
    /// <summary>将字节数格式化为人类可读的大小字符串</summary>
    /// <param name="bytes">字节数</param>
    /// <returns>格式化后的字符串，如 "1.5 GB"</returns>
    public static string FormatSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
