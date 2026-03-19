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

    [ObservableProperty]
    private ObservableCollection<SelectableItem<ContextMenuItem>> _selectableItems = [];

    [ObservableProperty]
    private int _selectedCount;

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

        SelectableItems = new ObservableCollection<SelectableItem<ContextMenuItem>>(
            filtered.Select(i => new SelectableItem<ContextMenuItem>(i)));
        FilteredItems = new ObservableCollection<ContextMenuItem>(filtered);
        UpdateSelectedCount();
    }

    private void UpdateSelectedCount()
    {
        SelectedCount = SelectableItems.Count(i => i.IsSelected);
    }

    [RelayCommand]
    private void ToggleSelectAll()
    {
        bool allSelected = SelectableItems.All(i => i.IsSelected);
        foreach (var item in SelectableItems)
            item.IsSelected = !allSelected;
        UpdateSelectedCount();
    }

    [RelayCommand]
    private async Task DisableSelectedAsync()
    {
        var selected = SelectableItems.Where(i => i.IsSelected).Select(i => i.Item).ToList();
        if (selected.Count == 0) return;

        if (!DialogHelper.Confirm(
            $"确定要禁用选中的 {selected.Count} 个右键菜单项吗？",
            "批量禁用确认"))
            return;

        await ExecuteOperationAsync(async progress =>
        {
            int total = selected.Count;
            for (int i = 0; i < total; i++)
            {
                progress.Report((int)((double)i / total * 100));
                await ContextMenuManager.DisableMenuItemAsync(selected[i]);
            }
            SetSuccess($"已禁用 {total} 个右键菜单项");
            await LoadItemsAsync();
        });
    }

    [RelayCommand]
    private async Task DeleteSelectedAsync()
    {
        var selected = SelectableItems.Where(i => i.IsSelected).Select(i => i.Item).ToList();
        if (selected.Count == 0) return;

        if (!DialogHelper.ConfirmDangerous(
            $"确定要删除选中的 {selected.Count} 个右键菜单项吗？\n\n此操作将从注册表中永久删除。",
            "批量删除确认"))
            return;

        await ExecuteOperationAsync(async progress =>
        {
            int total = selected.Count;
            for (int i = 0; i < total; i++)
            {
                progress.Report((int)((double)i / total * 100));
                await ContextMenuManager.DeleteMenuItemAsync(selected[i]);
            }
            SetSuccess($"已删除 {total} 个右键菜单项");
            await LoadItemsAsync();
        });
    }
}
