using CommunityToolkit.Mvvm.ComponentModel;

namespace DismSharp.UI.ViewModels;

/// <summary>ViewModel 基类，提供通用的加载和操作状态管理</summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private string _loadingStatus = "正在加载...";

    [ObservableProperty]
    private bool _isOperating;

    [ObservableProperty]
    private int _operationProgress;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isStatusError;

    /// <summary>开始加载操作</summary>
    protected void StartLoading(string status = "正在加载...")
    {
        IsLoading = true;
        LoadingStatus = status;
        StatusMessage = null;
    }

    /// <summary>结束加载操作</summary>
    protected void FinishLoading()
    {
        IsLoading = false;
    }

    /// <summary>开始操作（启用/禁用/删除等）</summary>
    protected void StartOperation()
    {
        IsOperating = true;
        OperationProgress = 0;
        StatusMessage = null;
    }

    /// <summary>结束操作</summary>
    protected void FinishOperation()
    {
        IsOperating = false;
    }

    /// <summary>设置成功消息</summary>
    protected void SetSuccess(string message)
    {
        StatusMessage = message;
        IsStatusError = false;
    }

    /// <summary>设置错误消息</summary>
    protected void SetError(string message)
    {
        StatusMessage = message;
        IsStatusError = true;
    }

    /// <summary>执行异步加载操作</summary>
    protected async Task ExecuteLoadAsync(Func<Task> loadAction, string loadingStatus = "正在加载...")
    {
        StartLoading(loadingStatus);
        try
        {
            await loadAction();
        }
        catch (Exception ex)
        {
            SetError($"加载失败: {ex.Message}");
        }
        finally
        {
            FinishLoading();
        }
    }

    /// <summary>执行异步操作</summary>
    protected async Task ExecuteOperationAsync(Func<IProgress<int>, Task> operationAction, string? successMessage = null, Func<Task>? onSuccess = null)
    {
        StartOperation();
        try
        {
            var progress = new Progress<int>(p => OperationProgress = p);
            await operationAction(progress);

            if (successMessage is not null)
                SetSuccess(successMessage);

            if (onSuccess is not null)
                await onSuccess();
        }
        catch (Exception ex)
        {
            SetError($"操作失败: {ex.Message}");
        }
        finally
        {
            FinishOperation();
        }
    }
}
