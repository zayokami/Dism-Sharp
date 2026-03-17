using System.ServiceProcess;

namespace DismSharp.Core.Helpers;

/// <summary>Windows 服务管理辅助类</summary>
public static class ServiceHelper
{
    /// <summary>停止指定的 Windows 服务</summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="timeout">超时时间</param>
    /// <returns>服务之前是否正在运行</returns>
    public static bool StopService(string serviceName, TimeSpan? timeout = null)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            if (sc.Status == ServiceControllerStatus.Stopped)
                return false;

            sc.Stop();
            sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout ?? TimeSpan.FromSeconds(30));
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>启动指定的 Windows 服务</summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="timeout">超时时间</param>
    public static void StartService(string serviceName, TimeSpan? timeout = null)
    {
        try
        {
            using var sc = new ServiceController(serviceName);
            if (sc.Status == ServiceControllerStatus.Running)
                return;

            sc.Start();
            sc.WaitForStatus(ServiceControllerStatus.Running, timeout ?? TimeSpan.FromSeconds(30));
        }
        catch
        {
            // 启动失败不抛异常
        }
    }
}
