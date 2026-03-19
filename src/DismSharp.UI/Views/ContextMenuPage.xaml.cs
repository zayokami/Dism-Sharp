using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class ContextMenuPage : Page
{
    public ContextMenuPage()
    {
        InitializeComponent();
        var vm = new ContextMenuViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadItemsCommand.ExecuteAsync(null);
    }
}
