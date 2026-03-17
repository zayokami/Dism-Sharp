namespace DismSharp.Core;

/// <summary>DISM API 操作失败时抛出的异常</summary>
public class DismSharpException : Exception
{
    /// <summary>创建 DISM 异常</summary>
    /// <param name="hResult">HRESULT 错误码</param>
    public DismSharpException(int hResult)
        : base($"DISM 操作失败，HRESULT: 0x{hResult:X8}")
    {
        HResult = hResult;
    }

    /// <summary>创建 DISM 异常</summary>
    /// <param name="hResult">HRESULT 错误码</param>
    /// <param name="message">错误消息</param>
    public DismSharpException(int hResult, string message)
        : base(message)
    {
        HResult = hResult;
    }

    /// <summary>创建 DISM 异常</summary>
    public DismSharpException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>检查 HRESULT，失败时抛出异常</summary>
    /// <param name="hResult">HRESULT 返回值</param>
    internal static void ThrowIfFailed(int hResult)
    {
        if (hResult < 0)
            throw new DismSharpException(hResult);
    }
}
