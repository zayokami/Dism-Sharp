using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core.Modules;
using DismSharp.UI.Helpers;
using DismSharp.UI.Services;
using Microsoft.Extensions.Logging;

namespace DismSharp.UI.ViewModels;

public partial class ContextMenuViewModel : ViewModelBase
{
    private static readonly ILogger<ContextMenuViewModel> _logger = LoggingService.GetLogger<ContextMenuViewModel>();

    private List<ContextMenuItem> _allItems = [];

    [ObservableProperty]
    private ObservableCollection<ContextMenuItem> _filteredItems = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private ContextMenuItem? _selectedItem;

    [ObservableProperty]
    private int _totalCount;

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        await ExecuteLoadAsync(async () =>
        {
            var items = await ContextMenuManager.GetContextMenuItemsAsync();
            _allItems = items;
            TotalCount = items.Count;
            ApplyFilter();
        }, "正在扫描右键菜单...");
    }

    [RelayCommand]
    private async Task DisableItemAsync(ContextMenuItem? item)
    {
        if (item is null) return;

        if (!DialogHelper.Confirm(
            $"确定要禁用右键菜单项 \"{item.Name}\" 吗？",
            "禁用确认"))
            return;

        await ExecuteOperationAsync(async progress =>
        {
            progress.Report(50);
            await ContextMenuManager.DisableMenuItemAsync(item);
            progress.Report(100);
            SetSuccess($"已禁用 {item.Name}");
            await LoadItemsAsync();
        });
    }

    [RelayCommand]
    private async Task EnableItemAsync(ContextMenuItem? item)
    {
        if (item is null) return;

        await ExecuteOperationAsync(async progress =>
        {
            progress.Report(50);
            await ContextMenuManager.EnableMenuItemAsync(item);
            progress.Report(100);
            SetSuccess($"已启用 {item.Name}");
            await LoadItemsAsync();
        });
    }

    [RelayCommand]
    private async Task DeleteItemAsync(ContextMenuItem? item)
    {
        if (item is null) return;

        if (!DialogHelper.ConfirmDangerous(
            $"确定要删除右键菜单项 \"{item.Name}\" 吗？\n\n此操作将从注册表中永久删除。",
            "删除确认"))
            return;

        await ExecuteOperationAsync(async progress =>
        {
            progress.Report(50);
            await ContextMenuManager.DeleteMenuItemAsync(item);
            progress.Report(100);
            SetSuccess($"已删除 {item.Name}");
            await LoadItemsAsync();
        });
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allItems
            : _allItems.Where(i =>
                i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                i.Command.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        FilteredItems = new ObservableCollection<ContextMenuItem>(filtered);
    }
}
