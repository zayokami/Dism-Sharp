using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.Core.Modules;
using DismSharp.Core.Native;
using DismSharp.UI.Helpers;

namespace DismSharp.UI.ViewModels;

/// <summary>驱动管理页面 ViewModel</summary>
public partial class DriversViewModel : ViewModelBase
{
    private List<DriverPackageInfo> _allDrivers = [];

    [ObservableProperty]
    private ObservableCollection<DriverPackageInfo> _filteredDrivers = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private bool _showAllDrivers;

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
        await ExecuteLoadAsync(async () =>
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
        }, "正在查询驱动列表...");
    }

    /// <summary>备份所有第三方驱动</summary>
    [RelayCommand]
    private async Task BackupDriversAsync()
    {
        await ExecuteOperationAsync(async progress =>
        {
            var backupDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"DismSharp_DriverBackup_{DateTime.Now:yyyyMMdd_HHmmss}");

            using var session = DismSharpSession.OpenOnline();
            var count = await DriverManager.BackupDriversAsync(session, backupDir, progress);

            SetSuccess($"已成功备份 {count} 个第三方驱动到 {backupDir}");
        });
    }

    /// <summary>删除第三方驱动</summary>
    [RelayCommand]
    private async Task RemoveDriverAsync(DriverPackageInfo? driver)
    {
        if (driver is null || driver.InBox) return;

        if (!DialogHelper.ConfirmDangerous(
            $"确定要删除驱动 \"{driver.PublishedName}\" 吗？\n\n删除后可能需要重新安装该驱动。",
            "删除确认"))
            return;

        var publishedName = driver.PublishedName;
        await ExecuteOperationAsync(async _ =>
        {
            using var session = DismSharpSession.OpenOnline();
            await DriverManager.RemoveDriverAsync(session, publishedName);
        }, $"已成功删除驱动 {publishedName}", LoadDriversAsync);
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
