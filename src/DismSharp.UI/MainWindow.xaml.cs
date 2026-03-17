using Wpf.Ui.Controls;

namespace DismSharp.UI;

/// <summary>主窗口</summary>
public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
        InitializeComponent();

        Loaded += (_, _) => RootNavigation.Navigate(typeof(Views.DashboardPage));
    }
}
