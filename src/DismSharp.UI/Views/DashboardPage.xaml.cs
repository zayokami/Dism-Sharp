using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        var vm = new DashboardViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadDataCommand.ExecuteAsync(null);
    }
}
