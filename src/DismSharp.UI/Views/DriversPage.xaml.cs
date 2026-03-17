using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class DriversPage : Page
{
    public DriversPage()
    {
        InitializeComponent();
        var vm = new DriversViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadDriversCommand.ExecuteAsync(null);
    }
}
