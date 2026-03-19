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

        // DISM API 将在首次使用时自动初始化（延迟加载）
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
