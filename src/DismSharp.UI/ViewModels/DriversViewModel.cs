using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.Core.Modules;
using DismSharp.Core.Native;

namespace DismSharp.UI.ViewModels;

/// <summary>驱动管理页面 ViewModel</summary>
public partial class DriversViewModel : ObservableObject
{
    private List<DriverPackageInfo> _allDrivers = [];

    [ObservableProperty]
    private ObservableCollection<DriverPackageInfo> _filteredDrivers = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private bool _showAllDrivers;

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private string _loadingStatus = "正在查询驱动列表...";

    [ObservableProperty]
    private bool _isOperating;

    [ObservableProperty]
    private int _operationProgress;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isStatusError;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _thirdPartyCount;

    [ObservableProperty]
    private int _signedCount;

    [ObservableProperty]
    private int _unsignedCount;

    [ObservableProperty]
    private DriverPackageInfo? _selectedDriver;

    /// <summary>加载驱动列表</summary>
    [RelayCommand]
    private async Task LoadDriversAsync()
    {
        IsLoading = true;
        LoadingStatus = "正在查询驱动列表...";

        try
        {
            var showAll = ShowAllDrivers;
            var drivers = await Task.Run(() =>
            {
                using var session = DismSharpSession.OpenOnline();
                return session.GetDrivers(allDrivers: showAll);
            });

            _allDrivers = drivers;
            TotalCount = drivers.Count;
            ThirdPartyCount = drivers.Count(d => !d.InBox);
            SignedCount = drivers.Count(d => d.DriverSignature == DismDriverSignature.DismDriverSignatureSigned);
            UnsignedCount = drivers.Count(d => d.DriverSignature == DismDriverSignature.DismDriverSignatureUnsigned);

            ApplyFilter();
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>备份所有第三方驱动</summary>
    [RelayCommand]
    private async Task BackupDriversAsync()
    {
        IsOperating = true;
        OperationProgress = 0;
        StatusMessage = null;

        try
        {
            // 备份到用户桌面的 DismSharp_DriverBackup 目录
            var backupDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"DismSharp_DriverBackup_{DateTime.Now:yyyyMMdd_HHmmss}");

            var progress = new Progress<int>(p => OperationProgress = p);

            using var session = DismSharpSession.OpenOnline();
            var count = await DriverManager.BackupDriversAsync(session, backupDir, progress);

            StatusMessage = $"已成功备份 {count} 个第三方驱动到 {backupDir}";
            IsStatusError = false;
        }
        catch (Exception ex)
        {
            StatusMessage = $"备份失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsOperating = false;
        }
    }

    /// <summary>删除第三方驱动</summary>
    [RelayCommand]
    private async Task RemoveDriverAsync(DriverPackageInfo? driver)
    {
        if (driver is null || driver.InBox) return;

        IsOperating = true;
        StatusMessage = null;

        try
        {
            using var session = DismSharpSession.OpenOnline();
            await DriverManager.RemoveDriverAsync(session, driver.PublishedName);

            StatusMessage = $"已成功删除驱动 {driver.PublishedName}";
            IsStatusError = false;

            await LoadDriversAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"删除驱动失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsOperating = false;
        }
    }

    partial void OnShowAllDriversChanged(bool value) => LoadDriversCommand.Execute(null);

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    /// <summary>根据搜索文本过滤驱动列表</summary>
    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allDrivers
            : _allDrivers.Where(d =>
                d.PublishedName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                d.ClassName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                d.ProviderName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        FilteredDrivers = new ObservableCollection<DriverPackageInfo>(filtered);
    }
}
