using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.UI.Helpers;
using DismSharp.UI.Services;
using Microsoft.Extensions.Logging;

namespace DismSharp.UI.ViewModels;

public partial class AppxViewModel : ViewModelBase
{
    private static readonly ILogger<AppxViewModel> _logger = LoggingService.GetLogger<AppxViewModel>();

    private List<DismSharpSession.AppxPackageInfo> _allPackages = [];

    [ObservableProperty]
    private ObservableCollection<DismSharpSession.AppxPackageInfo> _filteredPackages = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private DismSharpSession.AppxPackageInfo? _selectedPackage;

    [ObservableProperty]
    private int _totalCount;

    [RelayCommand]
    private async Task LoadPackagesAsync()
    {
        await ExecuteLoadAsync(async () =>
        {
            var packages = await Task.Run(() =>
            {
                using var session = DismSharpSession.OpenOnline();
                return session.GetAppxPackages();
            });
            _allPackages = packages;
            TotalCount = packages.Count;
            ApplyFilter();
        }, "正在查询 Appx 包列表...");
    }

    [RelayCommand]
    private async Task RemovePackageAsync(DismSharpSession.AppxPackageInfo? package)
    {
        if (package is null) return;

        if (!DialogHelper.ConfirmDangerous(
            $"确定要删除预配包 \"{package.DisplayName}\" 吗？\n\n删除后可能无法恢复某些应用功能。",
            "删除确认"))
            return;

        await ExecuteOperationAsync(async _ =>
        {
            using var session = DismSharpSession.OpenOnline();
            session.RemoveAppxPackage(package.PackageName);
            SetSuccess($"已成功删除 {package.DisplayName}");
            await LoadPackagesAsync();
        });
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allPackages
            : _allPackages.Where(p =>
                p.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                p.PackageName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        FilteredPackages = new ObservableCollection<DismSharpSession.AppxPackageInfo>(filtered);
    }
}
