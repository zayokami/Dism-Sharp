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
}
