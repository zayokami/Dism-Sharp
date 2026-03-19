using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class AppxPage : Page
{
    public AppxPage()
    {
        InitializeComponent();
        var vm = new AppxViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadPackagesCommand.ExecuteAsync(null);
    }
}
