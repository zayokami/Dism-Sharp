using System.Windows.Controls;
using DismSharp.UI.ViewModels;

namespace DismSharp.UI.Views;

public partial class CleanupPage : Page
{
    public CleanupPage()
    {
        InitializeComponent();
        DataContext = new CleanupViewModel();
    }
}
