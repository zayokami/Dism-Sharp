using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DismSharp.Core.CleanupRules;
using DismSharp.Core.Modules;
using DismSharp.UI.Helpers;

namespace DismSharp.UI.ViewModels;

/// <summary>清理规则显示项</summary>
public partial class CleanupRuleItem : ObservableObject
{
    public CleanupScanResult ScanResult { get; }

    public string Name => ScanResult.Rule.Name;
    public string Description => ScanResult.Rule.Description;
    public int FileCount => ScanResult.Entries.Count;
    public string SizeText => CleanupEngine.FormatBytes(ScanResult.TotalBytes);
    public long TotalBytes => ScanResult.TotalBytes;

    [ObservableProperty]
    private bool _isSelected = true;

    public CleanupRuleItem(CleanupScanResult scanResult)
    {
        ScanResult = scanResult;
        _isSelected = scanResult.IsSelected;
    }

    partial void OnIsSelectedChanged(bool value)
    {
        ScanResult.IsSelected = value;
    }
}

/// <summary>系统清理页面 ViewModel</summary>
public partial class CleanupViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<CleanupRuleItem> _ruleItems = [];

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _isCleaning;

    [ObservableProperty]
    private bool _hasScanned;

    [ObservableProperty]
    private string _scanStatus = "";

    [ObservableProperty]
    private int _operationProgress;

    [ObservableProperty]
    private string _totalSizeText = "0 B";

    [ObservableProperty]
    private int _totalFileCount;

    private List<CleanupScanResult>? _scanResults;

    /// <summary>扫描可清理项目</summary>
    [RelayCommand]
    private async Task ScanAsync()
    {
        IsScanning = true;
        HasScanned = false;
        StatusMessage = null;
        RuleItems.Clear();

        try
        {
            var rules = CleanupEngine.GetDefaultRules();
            var progress = new Progress<(string ruleName, int ruleIndex, int totalRules)>(p =>
            {
                ScanStatus = $"正在扫描: {p.ruleName} ({p.ruleIndex + 1}/{p.totalRules})";
                OperationProgress = (int)((double)(p.ruleIndex + 1) / p.totalRules * 100);
            });

            _scanResults = await CleanupEngine.ScanAsync(rules, progress);

            var items = new ObservableCollection<CleanupRuleItem>();
            foreach (var result in _scanResults)
            {
                items.Add(new CleanupRuleItem(result));
            }
            RuleItems = items;

            UpdateTotals();
            HasScanned = true;
            ScanStatus = "";
        }
        catch (Exception ex)
        {
            StatusMessage = $"扫描失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsScanning = false;
        }
    }

    /// <summary>执行清理</summary>
    [RelayCommand]
    private async Task CleanAsync()
    {
        if (_scanResults is null) return;

        var selectedCount = RuleItems.Count(r => r.IsSelected);
        var totalSize = RuleItems.Where(r => r.IsSelected).Sum(r => r.TotalBytes);
        if (selectedCount == 0) return;

        if (!DialogHelper.ConfirmDangerous(
            $"即将清理 {selectedCount} 个项目，预计释放 {CleanupEngine.FormatBytes(totalSize)} 空间。\n\n此操作不可撤销，确定要继续吗？",
            "清理确认"))
            return;

        IsCleaning = true;
        OperationProgress = 0;
        StatusMessage = null;

        try
        {
            // 同步选中状态
            foreach (var item in RuleItems)
            {
                item.ScanResult.IsSelected = item.IsSelected;
            }

            var progress = new Progress<(string ruleName, int ruleIndex, int totalRules, int cleanedFiles)>(p =>
            {
                ScanStatus = $"正在清理: {p.ruleName} ({p.ruleIndex + 1}/{p.totalRules})";
                OperationProgress = (int)((double)(p.ruleIndex + 1) / p.totalRules * 100);
            });

            long cleanedBytes = await CleanupEngine.CleanAsync(_scanResults, progress);

            StatusMessage = $"清理完成！已释放 {CleanupEngine.FormatBytes(cleanedBytes)} 空间";
            IsStatusError = false;
            ScanStatus = "";

            // 清理后重新扫描
            await ScanAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"清理失败: {ex.Message}";
            IsStatusError = true;
        }
        finally
        {
            IsCleaning = false;
        }
    }

    /// <summary>全选/取消全选</summary>
    [RelayCommand]
    private void ToggleSelectAll()
    {
        bool allSelected = RuleItems.All(r => r.IsSelected);
        foreach (var item in RuleItems)
        {
            item.IsSelected = !allSelected;
        }
        UpdateTotals();
    }

    private void UpdateTotals()
    {
        long totalBytes = RuleItems.Where(r => r.IsSelected).Sum(r => r.TotalBytes);
        TotalFileCount = RuleItems.Where(r => r.IsSelected).Sum(r => r.FileCount);
        TotalSizeText = CleanupEngine.FormatBytes(totalBytes);
    }
}
