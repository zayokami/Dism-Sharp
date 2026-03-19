using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace DismSharp.UI.Controls;

public partial class StatusBar : UserControl
{
    public static readonly DependencyProperty StatusMessageProperty =
        DependencyProperty.Register(nameof(StatusMessage), typeof(string), typeof(StatusBar), new PropertyMetadata(null));

    public static readonly DependencyProperty IsStatusErrorProperty =
        DependencyProperty.Register(nameof(IsStatusError), typeof(bool), typeof(StatusBar), new PropertyMetadata(false));

    public static readonly DependencyProperty IsOperatingProperty =
        DependencyProperty.Register(nameof(IsOperating), typeof(bool), typeof(StatusBar), new PropertyMetadata(false));

    public static readonly DependencyProperty OperationProgressProperty =
        DependencyProperty.Register(nameof(OperationProgress), typeof(int), typeof(StatusBar), new PropertyMetadata(0));

    public string? StatusMessage
    {
        get => (string?)GetValue(StatusMessageProperty);
        set => SetValue(StatusMessageProperty, value);
    }

    public bool IsStatusError
    {
        get => (bool)GetValue(IsStatusErrorProperty);
        set => SetValue(IsStatusErrorProperty, value);
    }

    public bool IsOperating
    {
        get => (bool)GetValue(IsOperatingProperty);
        set => SetValue(IsOperatingProperty, value);
    }

    public int OperationProgress
    {
        get => (int)GetValue(OperationProgressProperty);
        set => SetValue(OperationProgressProperty, value);
    }

    public InfoBarSeverity Severity => IsStatusError ? InfoBarSeverity.Error : InfoBarSeverity.Informational;

    public StatusBar()
    {
        InitializeComponent();
    }
}
