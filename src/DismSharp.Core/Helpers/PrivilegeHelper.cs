using System.Security.Principal;

namespace DismSharp.Core.Helpers;

/// <summary>权限检查辅助工具</summary>
public static class PrivilegeHelper
{
    /// <summary>检查当前进程是否以管理员身份运行</summary>
    public static bool IsRunningAsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
