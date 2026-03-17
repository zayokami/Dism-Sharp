using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.Core.Modules;

namespace DismSharp.UI.ViewModels;

/// <summary>更新包管理页面 ViewModel</summary>
public partial class PackagesViewModel : ObservableObject
{
    private List<PackageBasicInfo> _allPackages = [];

    [ObservableProperty]
    private ObservableCollection<PackageBasicInfo> _filteredPackages = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private string _loadingStatus = "正在查询更新包列表...";

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
    private PackageBasicInfo? _selectedPackage;

    /// <summary>加载已安装的更新包</summary>
    [RelayCommand]
    private async Task LoadPackagesAsync()
    {
        IsLoading = true;
        LoadingStatus = "正在查询更新包列表...";

        try
        {
            var packages = await Task.Run(() =>
            {
                using var session = DismSharpSession.OpenOnline();
                return session.GetPackages();
            });

            _allPackages = packages;
            TotalCount = packages.Count;
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

    /// <summary>卸载选中的更新包</summary>
    [RelayCommand]
    private async Task UninstallPackageAsync(PackageBasicInfo? package)
    {
        if (package is null) return;

        IsOperating = true;
        OperationProgress = 0;
        StatusMessage = null;

        try
        {
            var progress = new Progress<int>(p => OperationProgress = p);
            var name = package.PackageName;

            using var session = DismSharpSession.OpenOnline();
            await PackageManager.UninstallPackageAsync(session, name, progress);

            StatusMessage = $"已成功卸载 {name}，可能需要重新启动才能完成";
            IsStatusError = false;

            await LoadPackagesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"卸载失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsOperating = false;
        }
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
