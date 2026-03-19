using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class AppxPage : System.Windows.Controls.Page
{
    public AppxPage(AppxViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        InitializeComponent();

        Loaded += async (_, _) => await ViewModel.LoadPackagesCommand.ExecuteAsync(null);
    }

    public AppxViewModel ViewModel { get; }
}
