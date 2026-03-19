using System.Windows;
using DismSharp.Core;
using DismSharp.Core.Helpers;
using DismSharp.Core.Native;
using DismSharp.UI.Services;
using Serilog;

namespace DismSharp.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 初始化日志
        LoggingService.Initialize();
        Log.Information("Dism# 启动");

        // 管理员权限检查
        if (!PrivilegeHelper.IsRunningAsAdmin())
        {
            MessageBox.Show("请右键以管理员身份运行 Dism#！", "权限不足",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        // 初始化 DISM API
        try
        {
            DismSharpSession.Initialize(DismLogLevel.DismLogErrors);
            Log.Information("DISM API 初始化成功");
        }
        catch (DismSharpException ex)
        {
            Log.Error(ex, "DISM 初始化失败");
            MessageBox.Show($"DISM 初始化失败: {ex.Message}", "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            DismSharpSession.Shutdown();
            Log.Information("DISM API 已关闭");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DISM 关闭失败");
        }

        LoggingService.Shutdown();
        base.OnExit(e);
    }
}
