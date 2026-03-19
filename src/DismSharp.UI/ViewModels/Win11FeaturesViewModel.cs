using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core.Modules;
using DismSharp.UI.Helpers;
using DismSharp.UI.Services;
using Microsoft.Extensions.Logging;

namespace DismSharp.UI.ViewModels;

public partial class Win11FeaturesViewModel : ViewModelBase
{
    private static readonly ILogger<Win11FeaturesViewModel> _logger = LoggingService.GetLogger<Win11FeaturesViewModel>();
    private bool _isLoadingData;

    [ObservableProperty]
    private ObservableCollection<WsaAppInfo> _wsaApps = [];

    [ObservableProperty]
    private ObservableCollection<WslDistroInfo> _wslDistros = [];

    [ObservableProperty]
    private bool _isWsaInstalled;

    [ObservableProperty]
    private bool _isWidgetsEnabled;

    [ObservableProperty]
    private bool _isWidgetsTaskbarEnabled;

    [ObservableProperty]
    private bool _isCopilotEnabled;

    [ObservableProperty]
    private bool _isCopilotTaskbarEnabled;

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        _isLoadingData = true;
        try
        {
            await ExecuteLoadAsync(async () =>
            {
                var wsaTask = WsaManager.IsWsaInstalledAsync();
                var wsaAppsTask = WsaManager.GetInstalledAppsAsync();
                var wslTask = WslManager.GetDistrosAsync();

                await Task.WhenAll(wsaTask, wsaAppsTask, wslTask);

                IsWsaInstalled = wsaTask.Result;
                WsaApps = new ObservableCollection<WsaAppInfo>(wsaAppsTask.Result);
                WslDistros = new ObservableCollection<WslDistroInfo>(wslTask.Result);

                IsWidgetsEnabled = WidgetsManager.IsEnabled();
                IsWidgetsTaskbarEnabled = WidgetsManager.IsTaskbarButtonEnabled();
                IsCopilotEnabled = CopilotManager.IsEnabled();
                IsCopilotTaskbarEnabled = CopilotManager.IsTaskbarButtonEnabled();
            }, "正在加载 Win11 功能...");
        }
        finally
        {
            _isLoadingData = false;
        }
    }

    partial void OnIsWidgetsEnabledChanged(bool value)
    {
        if (_isLoadingData) return;
        WidgetsManager.SetEnabled(value);
        SetSuccess($"小组件已{(value ? "启用" : "禁用")}，可能需要重启资源管理器");
    }

    partial void OnIsWidgetsTaskbarEnabledChanged(bool value)
    {
        if (_isLoadingData) return;
        WidgetsManager.SetTaskbarButtonEnabled(value);
        SetSuccess($"任务栏小组件按钮已{(value ? "显示" : "隐藏")}");
    }

    partial void OnIsCopilotEnabledChanged(bool value)
    {
        if (_isLoadingData) return;
        CopilotManager.SetEnabled(value);
        SetSuccess($"Copilot 已{(value ? "启用" : "禁用")}，可能需要重启");
    }

    partial void OnIsCopilotTaskbarEnabledChanged(bool value)
    {
        if (_isLoadingData) return;
        CopilotManager.SetTaskbarButtonEnabled(value);
        SetSuccess($"任务栏 Copilot 按钮已{(value ? "显示" : "隐藏")}");
    }

    [RelayCommand]
    private async Task TerminateWslAsync(WslDistroInfo? distro)
    {
        if (distro is null) return;

        await ExecuteOperationAsync(async progress =>
        {
            progress.Report(50);
            await WslManager.ShutdownDistroAsync(distro.Name);
            progress.Report(100);
            SetSuccess($"已终止 {distro.Name}");
            await LoadDataAsync();
        });
    }

    [RelayCommand]
    private async Task SetDefaultWslAsync(WslDistroInfo? distro)
    {
        if (distro is null) return;

        await ExecuteOperationAsync(async progress =>
        {
            progress.Report(50);
            await WslManager.SetDefaultDistroAsync(distro.Name);
            progress.Report(100);
            SetSuccess($"已将 {distro.Name} 设为默认发行版");
            await LoadDataAsync();
        });
    }

    [RelayCommand]
    private async Task UninstallWslAsync(WslDistroInfo? distro)
    {
        if (distro is null) return;

        if (!DialogHelper.ConfirmDangerous(
            $"确定要卸载 WSL 发行版 \"{distro.Name}\" 吗？\n\n此操作将删除该发行版的所有数据，不可撤销。",
            "卸载确认"))
            return;

        await ExecuteOperationAsync(async progress =>
        {
            progress.Report(50);
            await WslManager.UninstallDistroAsync(distro.Name);
            progress.Report(100);
            SetSuccess($"已卸载 {distro.Name}");
            await LoadDataAsync();
        });
    }

    [RelayCommand]
    private async Task UninstallWsaAppAsync(WsaAppInfo? app)
    {
        if (app is null) return;

        if (!DialogHelper.ConfirmDangerous(
            $"确定要卸载 Android 应用 \"{app.DisplayName}\" 吗？",
            "卸载确认"))
            return;

        await ExecuteOperationAsync(async progress =>
        {
            progress.Report(50);
            await WsaManager.UninstallAppAsync(app.PackageName);
            progress.Report(100);
            SetSuccess($"已卸载 {app.DisplayName}");
            await LoadDataAsync();
        });
    }
}
