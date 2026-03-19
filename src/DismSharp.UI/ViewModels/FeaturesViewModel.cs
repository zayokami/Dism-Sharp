using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.Core.Modules;
using DismSharp.Core.Native;
using DismSharp.UI.Helpers;

namespace DismSharp.UI.ViewModels;

/// <summary>功能显示项</summary>
public partial class FeatureDisplayItem : ObservableObject
{
    [ObservableProperty]
    private string _featureName = "";

    [ObservableProperty]
    private DismPackageFeatureState _state;

    /// <summary>是否为已启用状态</summary>
    public bool IsEnabled => State == DismPackageFeatureState.DismStateInstalled;
}

/// <summary>Windows 功能管理页面 ViewModel</summary>
public partial class FeaturesViewModel : ViewModelBase
{
    private List<FeatureDisplayItem> _allFeatures = [];

    [ObservableProperty]
    private ObservableCollection<FeatureDisplayItem> _filteredFeatures = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private FeatureDisplayItem? _selectedFeature;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _enabledCount;

    /// <summary>加载所有 Windows 功能</summary>
    [RelayCommand]
    private async Task LoadFeaturesAsync()
    {
        await ExecuteLoadAsync(async () =>
        {
            var features = await Task.Run(() =>
            {
                using var session = DismSharpSession.OpenOnline();
                return session.GetFeatures();
            });

            _allFeatures = features.Select(f => new FeatureDisplayItem
            {
                FeatureName = f.FeatureName,
                State = f.State
            }).ToList();

            TotalCount = _allFeatures.Count;
            EnabledCount = _allFeatures.Count(f => f.IsEnabled);
            ApplyFilter();
        }, "正在查询 DISM 功能列表...");
    }

    /// <summary>启用选中的功能</summary>
    [RelayCommand]
    private async Task EnableFeatureAsync(FeatureDisplayItem? feature)
    {
        if (feature is null) return;

        var name = feature.FeatureName;
        await ExecuteOperationAsync(async progress =>
        {
            using var session = DismSharpSession.OpenOnline();
            await FeatureManager.EnableFeatureAsync(session, name, progress: progress);
        }, $"已成功启用 {name}，可能需要重新启动才能完成", LoadFeaturesAsync);
    }

    /// <summary>禁用选中的功能</summary>
    [RelayCommand]
    private async Task DisableFeatureAsync(FeatureDisplayItem? feature)
    {
        if (feature is null) return;

        if (!DialogHelper.Confirm(
            $"确定要禁用功能 \"{feature.FeatureName}\" 吗？\n\n禁用后需要重新启用才能使用该功能。",
            "禁用确认"))
            return;

        var name = feature.FeatureName;
        await ExecuteOperationAsync(async progress =>
        {
            using var session = DismSharpSession.OpenOnline();
            await FeatureManager.DisableFeatureAsync(session, name, progress: progress);
        }, $"已成功禁用 {name}，可能需要重新启动才能完成", LoadFeaturesAsync);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    /// <summary>根据搜索文本过滤功能列表</summary>
    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allFeatures
            : _allFeatures.Where(f => f.FeatureName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        FilteredFeatures = new ObservableCollection<FeatureDisplayItem>(filtered);
    }
}
