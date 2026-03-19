using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.Core.Modules;

namespace DismSharp.UI.ViewModels;

/// <summary>更新包管理页面 ViewModel</summary>
public partial class PackagesViewModel : ViewModelBase
{
    private List<PackageBasicInfo> _allPackages = [];

    [ObservableProperty]
    private ObservableCollection<PackageBasicInfo> _filteredPackages = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private PackageBasicInfo? _selectedPackage;

    /// <summary>加载已安装的更新包</summary>
    [RelayCommand]
    private async Task LoadPackagesAsync()
    {
        await ExecuteLoadAsync(async () =>
        {
            var packages = await Task.Run(() =>
            {
                using var session = DismSharpSession.OpenOnline();
                return session.GetPackages();
            });

            _allPackages = packages;
            TotalCount = packages.Count;
            ApplyFilter();
        }, "正在查询更新包列表...");
    }

    /// <summary>卸载选中的更新包</summary>
    [RelayCommand]
    private async Task UninstallPackageAsync(PackageBasicInfo? package)
    {
        if (package is null) return;

        var name = package.PackageName;
        await ExecuteOperationAsync(async progress =>
        {
            using var session = DismSharpSession.OpenOnline();
            await PackageManager.UninstallPackageAsync(session, name, progress);
        }, $"已成功卸载 {name}，可能需要重新启动才能完成", LoadPackagesAsync);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    /// <summary>根据搜索文本过滤包列表</summary>
    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allPackages
            : _allPackages.Where(p => p.PackageName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        FilteredPackages = new ObservableCollection<PackageBasicInfo>(filtered);
    }
}
