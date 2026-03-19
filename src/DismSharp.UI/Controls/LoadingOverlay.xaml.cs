using System.Windows;
using System.Windows.Controls;

namespace DismSharp.UI.Controls;

public partial class LoadingOverlay : UserControl
{
    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingOverlay), new PropertyMetadata(false));

    public static readonly DependencyProperty LoadingStatusProperty =
        DependencyProperty.Register(nameof(LoadingStatus), typeof(string), typeof(LoadingOverlay), new PropertyMetadata("正在加载..."));

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string LoadingStatus
    {
        get => (string)GetValue(LoadingStatusProperty);
        set => SetValue(LoadingStatusProperty, value);
    }

    public LoadingOverlay()
    {
        InitializeComponent();
    }
}
