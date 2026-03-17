using System.Windows;
using DismSharp.Core;
using DismSharp.Core.Helpers;
using DismSharp.Core.Native;

namespace DismSharp.UI;

/// <summary>应用程序入口</summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 管理员权限检查（manifest 已要求提权，这里做双重检查）
        if (!PrivilegeHelper.IsRunningAsAdmin())
        {
            MessageBox.Show("请以管理员身份运行 Dism#！", "权限不足",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        // 初始化 DISM API
        try
        {
            DismSharpSession.Initialize(DismLogLevel.DismLogErrors);
        }
        catch (DismSharpException ex)
        {
            MessageBox.Show($"DISM 初始化失败: {ex.Message}", "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // 关闭 DISM API
        try
        {
            DismSharpSession.Shutdown();
        }
        catch
        {
            // 退出时忽略关闭错误
        }

        base.OnExit(e);
    }
}
