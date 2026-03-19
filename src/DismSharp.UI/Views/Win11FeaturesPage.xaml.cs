using System.Windows;
using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class Win11FeaturesPage : Page
{
    public Win11FeaturesPage()
    {
        InitializeComponent();
        var vm = new Win11FeaturesViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadDataCommand.ExecuteAsync(null);
    }

    private void ToggleWidgets_Checked(object sender, RoutedEventArgs e) => ToggleWidgets();
    private void ToggleWidgets_Unchecked(object sender, RoutedEventArgs e) => ToggleWidgets();

    private void ToggleWidgets()
    {
        if (DataContext is Win11FeaturesViewModel vm)
            vm.ToggleWidgetsCommand.Execute(null);
    }

    private void ToggleWidgetsTaskbar_Checked(object sender, RoutedEventArgs e) => ToggleWidgetsTaskbar();
    private void ToggleWidgetsTaskbar_Unchecked(object sender, RoutedEventArgs e) => ToggleWidgetsTaskbar();

    private void ToggleWidgetsTaskbar()
    {
        if (DataContext is Win11FeaturesViewModel vm)
            vm.ToggleWidgetsTaskbarCommand.Execute(null);
    }

    private void ToggleCopilot_Checked(object sender, RoutedEventArgs e) => ToggleCopilot();
    private void ToggleCopilot_Unchecked(object sender, RoutedEventArgs e) => ToggleCopilot();

    private void ToggleCopilot()
    {
        if (DataContext is Win11FeaturesViewModel vm)
            vm.ToggleCopilotCommand.Execute(null);
    }

    private void ToggleCopilotTaskbar_Checked(object sender, RoutedEventArgs e) => ToggleCopilotTaskbar();
    private void ToggleCopilotTaskbar_Unchecked(object sender, RoutedEventArgs e) => ToggleCopilotTaskbar();

    private void ToggleCopilotTaskbar()
    {
        if (DataContext is Win11FeaturesViewModel vm)
            vm.ToggleCopilotTaskbarCommand.Execute(null);
    }
}
