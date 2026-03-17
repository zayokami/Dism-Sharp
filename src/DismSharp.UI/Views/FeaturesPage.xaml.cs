using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class FeaturesPage : Page
{
    public FeaturesPage()
    {
        InitializeComponent();
        var vm = new FeaturesViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadFeaturesCommand.ExecuteAsync(null);
    }
}
