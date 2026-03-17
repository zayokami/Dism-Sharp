using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class PackagesPage : Page
{
    public PackagesPage()
    {
        InitializeComponent();
        var vm = new PackagesViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadPackagesCommand.ExecuteAsync(null);
    }
}
