using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core;
using DismSharp.Core.Modules;
using DismSharp.Core.Native;

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
public partial class FeaturesViewModel : ObservableObject
{
    private List<FeatureDisplayItem> _allFeatures = [];

    [ObservableProperty]
    private ObservableCollection<FeatureDisplayItem> _filteredFeatures = [];

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private FeatureDisplayItem? _selectedFeature;

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private string _loadingStatus = "正在查询 DISM 功能列表...";

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
    private int _enabledCount;

    /// <summary>加载所有 Windows 功能</summary>
    [RelayCommand]
    private async Task LoadFeaturesAsync()
    {
        IsLoading = true;
        LoadingStatus = "正在查询 DISM 功能列表...";
        StatusMessage = null;

        try
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

    /// <summary>启用选中的功能</summary>
    [RelayCommand]
    private async Task EnableFeatureAsync(FeatureDisplayItem? feature)
    {
        if (feature is null) return;

        IsOperating = true;
        OperationProgress = 0;
        StatusMessage = null;

        try
        {
            var progress = new Progress<int>(p => OperationProgress = p);
            var name = feature.FeatureName;

            // FeatureManager 内部已有 Task.Run，不再外层包裹
            using var session = DismSharpSession.OpenOnline();
            await FeatureManager.EnableFeatureAsync(session, name, progress: progress);

            StatusMessage = $"已成功启用 {name}，可能需要重新启动才能完成";
            IsStatusError = false;

            await LoadFeaturesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"启用 {feature.FeatureName} 失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsOperating = false;
        }
    }

    /// <summary>禁用选中的功能</summary>
    [RelayCommand]
    private async Task DisableFeatureAsync(FeatureDisplayItem? feature)
    {
        if (feature is null) return;

        IsOperating = true;
        OperationProgress = 0;
        StatusMessage = null;

        try
        {
            var progress = new Progress<int>(p => OperationProgress = p);
            var name = feature.FeatureName;

            using var session = DismSharpSession.OpenOnline();
            await FeatureManager.DisableFeatureAsync(session, name, progress: progress);

            StatusMessage = $"已成功禁用 {name}，可能需要重新启动才能完成";
            IsStatusError = false;

            await LoadFeaturesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"禁用 {feature.FeatureName} 失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsOperating = false;
        }
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
