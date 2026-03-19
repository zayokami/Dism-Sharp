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

    [ObservableProperty]
    private ObservableCollection<SelectableItem<DismSharpSession.AppxPackageInfo>> _selectablePackages = [];

    [ObservableProperty]
    private int _selectedCount;

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

        SelectablePackages = new ObservableCollection<SelectableItem<DismSharpSession.AppxPackageInfo>>(
            filtered.Select(p => new SelectableItem<DismSharpSession.AppxPackageInfo>(p)));
        FilteredPackages = new ObservableCollection<DismSharpSession.AppxPackageInfo>(filtered);
        UpdateSelectedCount();
    }

    private void UpdateSelectedCount()
    {
        SelectedCount = SelectablePackages.Count(p => p.IsSelected);
    }

    [RelayCommand]
    private void ToggleSelectAll()
    {
        bool allSelected = SelectablePackages.All(p => p.IsSelected);
        foreach (var item in SelectablePackages)
            item.IsSelected = !allSelected;
        UpdateSelectedCount();
    }

    [RelayCommand]
    private async Task DeleteSelectedAsync()
    {
        var selected = SelectablePackages.Where(p => p.IsSelected).Select(p => p.Item).ToList();
        if (selected.Count == 0) return;

        if (!DialogHelper.ConfirmDangerous(
            $"确定要删除选中的 {selected.Count} 个预配包吗？\n\n删除后可能无法恢复某些应用功能。",
            "批量删除确认"))
            return;

        await ExecuteOperationAsync(async progress =>
        {
            int total = selected.Count;
            for (int i = 0; i < total; i++)
            {
                progress.Report((int)((double)i / total * 100));
                using var session = DismSharpSession.OpenOnline();
                session.RemoveAppxPackage(selected[i].PackageName);
            }
            SetSuccess($"已成功删除 {total} 个预配包");
            await LoadPackagesAsync();
        });
    }
}
